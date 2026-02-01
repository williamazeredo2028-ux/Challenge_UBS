namespace Challenge_UBS.Application.DTOs;

public class PortfolioSummaryDto
{
    public List<string> Categories { get; set; } = new();

    public Dictionary<string, CategorySummaryDto> Summary { get; set; }
        = new();
}