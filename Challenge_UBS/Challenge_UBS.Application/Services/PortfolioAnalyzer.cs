using Challenge_UBS.Application.Models;
using Challenge_UBS.Domain.Entities;
using Challenge_UBS.Domain.Enums;
using Challenge_UBS.Domain.Rules;

namespace Challenge_UBS.Application.Services;

public class PortfolioAnalyzer
{
    private readonly IReadOnlyCollection<IRiskRule> _riskRules;

    public PortfolioAnalyzer(IEnumerable<IRiskRule> riskRules)
    {
        _riskRules = riskRules.ToList();
    }

    public PortfolioSummary Analyze(IEnumerable<Trade> trades)
    {
        var summary = new PortfolioSummary();

        foreach (var trade in trades)
        {
            var category = Classify(trade);
            summary.AddTrade(category, trade.ClientId, trade.Value);
        }

        return summary;
    }

    private RiskCategory Classify(Trade trade)
    {
        foreach (var rule in _riskRules)
        {
            if (rule.IsMatch(trade))
                return rule.Category;
        }

        throw new InvalidOperationException("No risk rule matched the trade.");
    }
}