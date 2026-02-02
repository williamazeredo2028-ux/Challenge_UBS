using Challenge_UBS.Domain.Enums;
using Challenge_UBS.Domain.Models;
using Challenge_UBS.Domain.Rules;
using Xunit;

namespace Challenge_UBS.Tests.Domain.Rules;

public class LowRiskRuleTests
{
    private readonly LowRiskRule _rule = new();

    [Fact]
    public void IsMatch_ShouldReturnTrue_WhenValueIsLessThanOneMillion()
    {
        // Arrange
        var trade = new Trade(
            value: 500_000m,
            clientSector: ClientSector.Public,
            clientId: "CLI001"
        );

        // Act
        var result = _rule.IsMatch(trade);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsMatch_ShouldReturnTrue_WhenValueIsLessThanOneMillion_RegardlessOfClientSector()
    {
        // Arrange
        var trade = new Trade(
            value: 999_999m,
            clientSector: ClientSector.Private,
            clientId: "CLI002"
        );

        // Act
        var result = _rule.IsMatch(trade);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsMatch_ShouldReturnFalse_WhenValueIsEqualToOneMillion()
    {
        // Arrange
        var trade = new Trade(
            value: 1_000_000m,
            clientSector: ClientSector.Public,
            clientId: "CLI001"
        );

        // Act
        var result = _rule.IsMatch(trade);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsMatch_ShouldReturnFalse_WhenValueIsGreaterThanOneMillion()
    {
        // Arrange
        var trade = new Trade(
            value: 1_500_000m,
            clientSector: ClientSector.Private,
            clientId: "CLI001"
        );

        // Act
        var result = _rule.IsMatch(trade);

        // Assert
        Assert.False(result);
    }
}