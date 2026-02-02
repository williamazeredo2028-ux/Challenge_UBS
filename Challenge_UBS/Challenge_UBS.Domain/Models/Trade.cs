using Challenge_UBS.Domain.Enums;

namespace Challenge_UBS.Domain.Models;

//Class used to represent a trade with client details and value.
public class Trade
{
    public decimal Value { get; }
    public ClientSector ClientSector { get; }
    public string ClientId { get; }

    public Trade(decimal value, ClientSector clientSector, string clientId)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(value),
                "Trade value must be greater than zero.");

        Value = value;
        ClientSector = clientSector;
        ClientId = clientId;
    }
}