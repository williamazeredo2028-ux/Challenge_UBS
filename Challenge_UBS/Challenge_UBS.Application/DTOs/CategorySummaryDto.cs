namespace Challenge_UBS.Application.DTOs;

public class CategorySummaryDto
{
    public int Count { get; set; }
    public decimal TotalValue { get; set; }
    public string TopClient { get; set; } = default!;
}