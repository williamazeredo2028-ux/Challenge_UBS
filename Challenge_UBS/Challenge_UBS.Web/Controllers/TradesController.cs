using Challenge_UBS.Application.DTOs;
using Challenge_UBS.Application.Services;
using Challenge_UBS.Domain.Entities;
using Challenge_UBS.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Challenge_UBS.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TradesController : ControllerBase
{
    private readonly RiskClassifier _riskClassifier;
    private readonly PortfolioAnalyzer _portfolioAnalyzer;
    private readonly ILogger<TradesController> _logger;

    public TradesController(
        RiskClassifier riskClassifier,
        PortfolioAnalyzer portfolioAnalyzer,
        ILogger<TradesController> logger)
    {
        _riskClassifier = riskClassifier;
        _portfolioAnalyzer = portfolioAnalyzer;
        _logger = logger;
    }

    // Part 1 – Classification
    [HttpPost("classify")]
    public ActionResult<ClassificationResponseDto> Classify(
        [FromBody] List<TradeRequestDto> trades)
    {
        if (trades == null || trades.Count == 0)
            return BadRequest("Trade list cannot be empty.");

        var result = new List<string>(trades.Count);

        foreach (var dto in trades)
        {
            var trade = MapToDomain(dto);
            var category = _riskClassifier.Classify(trade);

            result.Add(category.ToString());
        }

        return Ok(new ClassificationResponseDto
        {
            Categories = result
        });
    }

    // Part 2 – Portfolio Analyze
    [HttpPost("analyze")]
    public ActionResult<object> Analyze(
        [FromBody] List<TradeRequestDto> trades)
    {
        if (trades == null || trades.Count == 0)
            return BadRequest("Trade list cannot be empty.");

        if (trades.Count > 100_000)
            return BadRequest("Maximum allowed trades per request is 100,000.");

        var stopwatch = Stopwatch.StartNew();

        var domainTrades = trades.Select(MapToDomain).ToList();

        var portfolioSummary = _portfolioAnalyzer.Analyze(domainTrades);

        var response = new PortfolioSummaryDto
        {
            Categories = portfolioSummary.Categories.Keys
                .Select(c => c.ToString())
                .ToList(),

            Summary = portfolioSummary.Categories.ToDictionary(
                k => k.Key.ToString(),
                v => new CategorySummaryDto
                {
                    Count = v.Value.Count,
                    TotalValue = v.Value.TotalValue,
                    TopClient = v.Value.GetTopClient()
                })
        };

        stopwatch.Stop();

        return Ok(new
        {
            categories = response.Categories,
            summary = response.Summary,
            processingTimeMs = stopwatch.ElapsedMilliseconds
        });
    }

    // Mapping helper
    private static Trade MapToDomain(TradeRequestDto dto)
    {
        if (!Enum.TryParse<ClientSector>(
                dto.ClientSector,
                true,
                out var sector))
        {
            throw new ArgumentException(
                $"Invalid client sector: {dto.ClientSector}");
        }

        return new Trade(
            dto.Value,
            sector,
            dto.ClientId);
    }
}