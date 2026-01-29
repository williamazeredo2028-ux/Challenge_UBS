using System.Diagnostics;
using Challenge_UBS.Domain.Entities;
using Challenge_UBS.Domain.Enums;
using Challenge_UBS.Domain.Interfaces;

namespace Challenge_UBS.Application.Services;
public class RiskClassifier
{
    private readonly IEnumerable<IRiskRule> _rules;

    public RiskClassifier(IEnumerable<IRiskRule> rules)
    {
        _rules = rules;
    }

    public RiskCategory Classify(Trade trade)
    {
        //Take the first matching rule and return its category
        return _rules.First(r => r.IsMatch(trade)).Category;
    }
}