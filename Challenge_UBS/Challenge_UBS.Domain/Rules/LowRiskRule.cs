using Challenge_UBS.Domain.Models;
using Challenge_UBS.Domain.Enums;

namespace Challenge_UBS.Domain.Rules;

//Class used to classify Low Risk trades
public class LowRiskRule : IRiskRule
{
    public RiskCategory Category => RiskCategory.LOWRISK;

    public bool IsMatch(Trade trade)
    {
        return trade.Value < 1_000_000;
    }
}