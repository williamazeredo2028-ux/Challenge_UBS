using Challenge_UBS.Domain.Entities;
using System.Runtime.ConstrainedExecution;

namespace Challenge_UBS.Application.DTOs;

public class TradeRequestDto
{
    public decimal Value { get; set; }
    public string ClientSector { get; set; } = default!;
    public string ClientId { get; set; } = default!;
}