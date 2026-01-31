using Challenge_UBS.Domain.Entities;
using Challenge_UBS.Domain.Enums;

namespace Challenge_UBS.Domain.Rules;

public interface IRiskRule
{
    bool IsMatch(Trade trade);
    RiskCategory Category { get; }
}