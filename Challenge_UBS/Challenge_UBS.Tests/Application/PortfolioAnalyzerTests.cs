using Challenge_UBS.Application.Models;
using Challenge_UBS.Application.Services;
using Challenge_UBS.Domain.Enums;
using Challenge_UBS.Domain.Models;
using Challenge_UBS.Domain.Rules;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Challenge_UBS.Tests.Application.Services;

public class PortfolioAnalyzerTests
{
    // Helper to create a simple mock rule
    private static Mock<IRiskRule> CreateRuleMock(RiskCategory category, Func<Trade, bool> matchLogic)
    {
        var mock = new Mock<IRiskRule>();
        mock.Setup(r => r.Category).Returns(category);
        mock.Setup(r => r.IsMatch(It.IsAny<Trade>())).Returns<Trade>(matchLogic);
        return mock;
    }

    [Fact]
    public void Constructor_Throws_WhenRiskRulesIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new PortfolioAnalyzer(null!));
    }

    [Fact]
    public void Constructor_AcceptsEmptyCollection_ButWillFailLaterOnClassification()
    {
        var analyzer = new PortfolioAnalyzer(Enumerable.Empty<IRiskRule>());

        var trade = new Trade(1000000m, ClientSector.Private, "C001");

        Assert.Throws<InvalidOperationException>(() =>
            analyzer.Analyze(new[] { trade }));
    }

    [Fact]
    public void Analyze_ThrowsArgumentNullException_WhenTradesIsNull()
    {
        var rules = new List<IRiskRule> { CreateRuleMock(RiskCategory.LOWRISK, _ => true).Object };
        var analyzer = new PortfolioAnalyzer(rules);

        Assert.Throws<ArgumentNullException>(() => analyzer.Analyze(null!));
    }

    [Fact]
    public void Analyze_ReturnsEmptySummary_WhenNoTradesAreProvided()
    {
        var rules = new List<IRiskRule> { CreateRuleMock(RiskCategory.LOWRISK, _ => true).Object };
        var analyzer = new PortfolioAnalyzer(rules);

        var summary = analyzer.Analyze(Enumerable.Empty<Trade>());

        Assert.NotNull(summary);
        Assert.Empty(summary.Categories);
    }

    [Fact]
    public void Analyze_Throws_WhenAnyTradeHasNullOrEmptyClientId()
    {
        var rules = new List<IRiskRule> { CreateRuleMock(RiskCategory.LOWRISK, _ => true).Object };
        var analyzer = new PortfolioAnalyzer(rules);

        var trades = new[]
        {
            new Trade(500000m, ClientSector.Public, "C001"),
            new Trade(1500000m, ClientSector.Private, "")
        };

        var ex = Assert.Throws<ArgumentNullException>(() => analyzer.Analyze(trades));
        Assert.Contains("clientId", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Analyze_CallsAddTrade_WithCorrectCategoryAndValues()
    {
        // Arrange
        var lowRule = CreateRuleMock(RiskCategory.LOWRISK, t => t.Value < 1_000_000m);
        var highRule = CreateRuleMock(RiskCategory.HIGHRISK, t => t.Value >= 1_000_000m && t.ClientSector == ClientSector.Private);

        var rules = new List<IRiskRule> { lowRule.Object, highRule.Object };

        var analyzer = new PortfolioAnalyzer(rules);

        var trades = new[]
        {
            new Trade(800_000m, ClientSector.Private, "CLI-A"),
            new Trade(2_500_000m, ClientSector.Private, "CLI-B"),
            new Trade (400_000m, ClientSector.Public, "CLI-C")
        };

        // Act
        var summary = analyzer.Analyze(trades);

        // Assert
        Assert.Equal(2, summary.Categories.Count);
        Assert.True(summary.Categories.ContainsKey(RiskCategory.LOWRISK));
        Assert.True(summary.Categories.ContainsKey(RiskCategory.HIGHRISK));

        var low = summary.Categories[RiskCategory.LOWRISK];
        var high = summary.Categories[RiskCategory.HIGHRISK];

        Assert.Equal(2, low.Count);
        Assert.Equal(800000m + 400000m, low.TotalValue);
        Assert.Equal(1, high.Count);
        Assert.Equal(2500000m, high.TotalValue);
    }

    [Fact]
    public void Analyze_UsesFirstMatchingRule_WhenMultipleCouldMatch()
    {
        // Arrange - low risk rule is first and matches everything
        var alwaysLow = CreateRuleMock(RiskCategory.LOWRISK, _ => true);
        var highRule = CreateRuleMock(RiskCategory.HIGHRISK, _ => true);

        var analyzer = new PortfolioAnalyzer(new[] { alwaysLow.Object, highRule.Object });

        var trades = new[]
        {
            new Trade (5_000_000m, ClientSector.Private, "BIG")
        };

        // Act
        var summary = analyzer.Analyze(trades);

        // Assert - should be LOWRISK because first rule wins
        Assert.Single(summary.Categories);
        Assert.True(summary.Categories.ContainsKey(RiskCategory.LOWRISK));
        Assert.False(summary.Categories.ContainsKey(RiskCategory.HIGHRISK));

        alwaysLow.Verify(r => r.IsMatch(It.IsAny<Trade>()), Times.Once());
        highRule.Verify(r => r.IsMatch(It.IsAny<Trade>()), Times.Never());
    }

    [Fact]
    public void Classify_ThrowsInvalidOperationException_WhenNoRuleMatches()
    {
        var analyzer = new PortfolioAnalyzer(Enumerable.Empty<IRiskRule>());

        var trade = new Trade (1000000m, ClientSector.Public, "TEST");

        // Use reflection to reach private method
        var classifyMethod = typeof(PortfolioAnalyzer)
            .GetMethod("Classify", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);


        var genExcp = Assert.Throws<TargetInvocationException>(() =>
            classifyMethod!.Invoke(analyzer, new object[] { trade }));

        var innerExcp = Assert.IsType<InvalidOperationException>(genExcp.InnerException);

        Assert.Contains("No risk rule matched the trade.", innerExcp.Message);
    }
}