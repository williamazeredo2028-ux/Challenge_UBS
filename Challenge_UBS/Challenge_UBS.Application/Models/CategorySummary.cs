namespace Challenge_UBS.Application.Models;

public class CategorySummary
{
    public int Count { get; private set; }
    public decimal TotalValue { get; private set; }

    private readonly Dictionary<string, decimal> _clientExposure = new();

    public void AddTrade(string clientId, decimal value)
    {
        Count++;
        TotalValue += value;

        if (!_clientExposure.ContainsKey(clientId))
            _clientExposure[clientId] = 0;

        _clientExposure[clientId] += value;
    }

    public string GetTopClient()
        => _clientExposure
            .OrderByDescending(x => x.Value)
            .First()
            .Key;
}