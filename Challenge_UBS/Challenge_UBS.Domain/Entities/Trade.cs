namespace Challenge_UBS.Domain.Entities;

//Class used to represent a trade with client details and value.
public class Trade
{
    public string ClientId { get; init; } = default!;
    public decimal Value { get; init; }
    public string ClientSector { get; init; } = default!;
}