using Challenge_UBS.Domain.Enums;
using Challenge_UBS.Domain.Models;
using Challenge_UBS.Domain.Rules;
using Xunit;

namespace Challenge_UBS.Tests.Domain.Rules;

public class HighRiskRuleTests
{
    private readonly HighRiskRule _rule = new();

    [Fact]
    public void IsMatch_ShouldReturnTrue_WhenValueIsGreaterOrEqualToOneMillion_AndClientIsPrivate()
    {
        // Arrange
        var trade = new Trade(
            value: 2_000_000m,
            clientSector: ClientSector.Private,
            clientId: "CLI001"
        );

        // Act
        var result = _rule.IsMatch(trade);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsMatch_ShouldReturnFalse_WhenValueIsLessThanOneMillion()
    {
        // Arrange
        var trade = new Trade(
            value: 500_000m,
            clientSector: ClientSector.Private,
            clientId: "CLI001"
        );

        // Act
        var result = _rule.IsMatch(trade);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsMatch_ShouldReturnFalse_WhenClientSectorIsPublic()
    {
        // Arrange
        var trade = new Trade(
            value: 2_000_000m,
            clientSector: ClientSector.Public,
            clientId: "CLI001"
        );

        // Act
        var result = _rule.IsMatch(trade);

        // Assert
        Assert.False(result);
    }
}