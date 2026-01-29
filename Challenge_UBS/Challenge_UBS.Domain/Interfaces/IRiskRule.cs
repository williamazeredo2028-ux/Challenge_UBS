using Challenge_UBS.Domain.Entities;
using Challenge_UBS.Domain.Enums;
using System.Diagnostics;

namespace Challenge_UBS.Domain.Interfaces;

public interface IRiskRule
{
    bool IsMatch(Trade trade);
    RiskCategory Category { get; }
}