using Challenge_UBS.Domain.Entities;
using Challenge_UBS.Domain.Enums;

namespace Challenge_UBS.Domain.Rules;

//Class used to classify High Risk trades
public class HighRiskRule : IRiskRule
{
    public RiskCategory Category => RiskCategory.HIGHRISK;

    public bool IsMatch(Trade trade)
    {
        return trade.Value >= 1_000_000
            && trade.ClientSector == ClientSector.Private;
    }
}