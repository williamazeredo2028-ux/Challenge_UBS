using Challenge_UBS.Application.Models;
using Challenge_UBS.Domain.Enums;
using Challenge_UBS.Domain.Models;
using Challenge_UBS.Domain.Rules;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;

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
        ArgumentNullException.ThrowIfNull(trades);

        var summary = new PortfolioSummary();

        foreach (var trade in trades)
        {
            if (string.IsNullOrEmpty(trade.ClientId))
                throw new ArgumentNullException("Trade ClientId cannot be null or empty.", nameof(trades));

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