using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Challenge_UBS.Application.Services;
using Challenge_UBS.Application.DTOs;

namespace Challenge_UBS.Web.Controllers;

[ApiController]
[Route("api/trades")]
public class TradesController : ControllerBase
{
    private readonly RiskClassifier _classifier;
    private readonly ILogger<TradesController> _logger;

    public TradesController(
        RiskClassifier classifier,
        ILogger<TradesController> logger)
    {
        _classifier = classifier;
        _logger = logger;
    }

    //Method used to classify Risk of the trades
    [HttpPost("classify")]
    public IActionResult Classify(List<TradeRequestDto> trades)
    {
        var result = trades
            .Select(t => _classifier.Classify(t.ToEntity()).ToString())
            .ToList();

        return Ok(new { categories = result });
    }
}