using Challenge_UBS.Domain.Enums;

namespace Challenge_UBS.Application.Models;

public class PortfolioSummary
{
    private readonly Dictionary<RiskCategory, CategorySummary> _categories
        = new();

    public IReadOnlyDictionary<RiskCategory, CategorySummary> Categories
        => _categories;

    public void AddTrade(
        RiskCategory category,
        string clientId,
        decimal value)
    {
        if (!_categories.ContainsKey(category))
            _categories[category] = new CategorySummary();

        _categories[category].AddTrade(clientId, value);
    }
}