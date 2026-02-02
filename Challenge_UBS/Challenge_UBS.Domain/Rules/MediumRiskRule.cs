using Challenge_UBS.Domain.Models;
using Challenge_UBS.Domain.Enums;

namespace Challenge_UBS.Domain.Rules;

//Class used to classify Medium Risk trades
public class MediumRiskRule : IRiskRule
{
    public RiskCategory Category => RiskCategory.MEDIUMRISK;

    public bool IsMatch(Trade trade)
    {
        return trade.Value >= 1_000_000
            && trade.ClientSector == ClientSector.Public;
    }
}