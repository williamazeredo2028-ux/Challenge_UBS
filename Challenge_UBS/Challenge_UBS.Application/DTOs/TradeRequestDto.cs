using Challenge_UBS.Domain.Entities;

namespace Challenge_UBS.Application.DTOs;

public class TradeRequestDto
{
    public string ClientId { get; set; } = default!;
    public decimal Value { get; set; }
    public string ClientSector { get; set; } = default!;

    //Convert to Data Transfer Object from Entity
    public Trade ToEntity()
    => new()
    {
        ClientId = ClientId,
        Value = Value,
        ClientSector = ClientSector
    };
}