using Challenge_UBS.Domain.Models;
using Challenge_UBS.Domain.Enums;
using Challenge_UBS.Domain.Rules;

namespace Challenge_UBS.Application.Services;

public class RiskClassifier
{
    private readonly IReadOnlyCollection<IRiskRule> _rules;

    public RiskClassifier(IEnumerable<IRiskRule> rules)
    {
        _rules = rules.ToList();
    }

    //Take the first matching rule and return its category
    public RiskCategory Classify(Trade trade)
    {
        foreach (var rule in _rules)
        {
            if (rule.IsMatch(trade))
                return rule.Category;
        }

        throw new InvalidOperationException("No risk rule matched the trade.");
    }
}