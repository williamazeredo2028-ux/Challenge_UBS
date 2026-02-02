using Challenge_UBS.Application.Services;
using Challenge_UBS.Domain.Enums;
using Challenge_UBS.Domain.Models;
using Challenge_UBS.Domain.Rules;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Challenge_UBS.Tests.Application.Services;

public class RiskClassifierTests
{
    private static Mock<IRiskRule> CreateRuleMock(
        RiskCategory category,
        Func<Trade, bool>? matchLogic = null)
    {
        var mock = new Mock<IRiskRule>();
        mock.Setup(r => r.Category).Returns(category);
        if (matchLogic != null)
        {
            mock.Setup(r => r.IsMatch(It.IsAny<Trade>())).Returns<Trade>(matchLogic);
        }
        return mock;
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenRulesIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RiskClassifier(null!));
    }

    [Fact]
    public void Constructor_AcceptsEmptyCollection()
    {
        var classifier = new RiskClassifier(Array.Empty<IRiskRule>());

        var trade = CreateValidTrade(500_000m, ClientSector.Public);

        Assert.Throws<InvalidOperationException>(() =>
            classifier.Classify(trade));
    }

    [Fact]
    public void Classify_ThrowsInvalidOperationException_WhenNoRuleMatches()
    {
        var classifier = new RiskClassifier(Array.Empty<IRiskRule>());

        var trade = CreateValidTrade(1_200_000m, ClientSector.Private);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            classifier.Classify(trade));

        Assert.Contains("No risk rule matched the trade", ex.Message);
    }

    [Theory]
    [InlineData(999_999, ClientSector.Public, RiskCategory.LOWRISK)]
    [InlineData(999_999, ClientSector.Private, RiskCategory.LOWRISK)]
    [InlineData(1_000_000, ClientSector.Public, RiskCategory.MEDIUMRISK)]
    [InlineData(1_000_000, ClientSector.Private, RiskCategory.HIGHRISK)]
    [InlineData(5_000_000, ClientSector.Public, RiskCategory.MEDIUMRISK)]
    [InlineData(5_000_000, ClientSector.Private, RiskCategory.HIGHRISK)]
    public void Classify_ReturnsCorrectCategory_WhenRulesAreOrderedCorrectly(
        decimal value,
        ClientSector clientSector,
        RiskCategory expectedCategory)
    {
        // Arrange - realistic rule order: LOW first, then MEDIUM, then HIGH
        var rules = new List<IRiskRule>
        {
            CreateRuleMock(RiskCategory.LOWRISK, t => t.Value < 1_000_000m).Object,
            CreateRuleMock(RiskCategory.MEDIUMRISK, t => t.Value >= 1_000_000m && t.ClientSector == ClientSector.Public).Object,
            CreateRuleMock(RiskCategory.HIGHRISK, t => t.Value >= 1_000_000m && t.ClientSector == ClientSector.Private).Object
        };

        var classifier = new RiskClassifier(rules);

        var trade = CreateValidTrade(value, clientSector);

        // Act
        var result = classifier.Classify(trade);

        // Assert
        Assert.Equal(expectedCategory, result);
    }

    [Fact]
    public void Classify_ReturnsFirstMatchingRule_WhenMultipleRulesCouldMatch()
    {
        // Arrange: greedy low-risk rule first
        var greedyLow = CreateRuleMock(RiskCategory.LOWRISK, _ => true);
        var highRule = CreateRuleMock(RiskCategory.HIGHRISK, _ => true);

        var classifier = new RiskClassifier(new[]
        {
            greedyLow.Object,
            highRule.Object
        });

        var trade = CreateValidTrade(10_000_000m, ClientSector.Private);

        // Act
        var result = classifier.Classify(trade);

        // Assert
        Assert.Equal(RiskCategory.LOWRISK, result);

        // Verify behavior
        greedyLow.Verify(r => r.IsMatch(It.IsAny<Trade>()), Times.Once());
        highRule.Verify(r => r.IsMatch(It.IsAny<Trade>()), Times.Never());
    }

    [Fact]
    public void Classify_DoesNotCallSubsequentRules_AfterFirstMatch()
    {
        // Arrange
        var lowRule = CreateRuleMock(RiskCategory.LOWRISK, t => t.Value < 1_000_000m);
        var mediumRule = CreateRuleMock(RiskCategory.MEDIUMRISK, _ => true);
        var highRule = CreateRuleMock(RiskCategory.HIGHRISK, _ => true);

        var classifier = new RiskClassifier(new[]
        {
            lowRule.Object,
            mediumRule.Object,
            highRule.Object
        });

        var trade = CreateValidTrade(750_000m, ClientSector.Private);

        // Act
        var result = classifier.Classify(trade);

        // Assert
        Assert.Equal(RiskCategory.LOWRISK, result);

        lowRule.Verify(r => r.IsMatch(It.IsAny<Trade>()), Times.Once());
        mediumRule.Verify(r => r.IsMatch(It.IsAny<Trade>()), Times.Never());
        highRule.Verify(r => r.IsMatch(It.IsAny<Trade>()), Times.Never());
    }

    [Fact]
    public void Classify_Throws_WhenTradeIsNull()
    {
        var rules = new[] { CreateRuleMock(RiskCategory.LOWRISK, _ => true).Object };
        var classifier = new RiskClassifier(rules);

        Assert.Throws<ArgumentNullException>(() =>
            classifier.Classify(null!));
    }

    // Helper(s)

    private static Trade CreateValidTrade(decimal value, ClientSector clientSector)
    {
        return new Trade
        (
            value,
            clientSector,
            "TEST-CLIENT"
        );
    }
}