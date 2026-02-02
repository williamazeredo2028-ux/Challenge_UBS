using Challenge_UBS.Domain.Models;
using Challenge_UBS.Domain.Enums;

namespace Challenge_UBS.Domain.Rules;

public interface IRiskRule
{
    bool IsMatch(Trade trade);
    RiskCategory Category { get; }
}