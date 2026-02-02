using Challenge_UBS.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Challenge_UBS.Web;

namespace Challenge_UBS.Tests.Integration.Controllers;

public partial class TradesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TradesControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Classify_ReturnsBadRequest_WhenEmptyListIsSent()
    {
        // Arrange
        var emptyList = new List<TradeRequestDto>();

        // Act
        var response = await _client.PostAsJsonAsync("/api/trades/classify", emptyList);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Trade list cannot be empty", content);
    }

    [Fact]
    public async Task Classify_ReturnsOk_WithCorrectCategories_ForValidTrades()
    {
        // Arrange
        var trades = new List<TradeRequestDto>
        {
            new() { Value = 800_000m,   ClientSector = "Private", ClientId = "CLI001" }, // LOWRISK
            new() { Value = 1_200_000m, ClientSector = "Public",  ClientId = "CLI002" }, // MEDIUMRISK
            new() { Value = 2_500_000m, ClientSector = "Private", ClientId = "CLI003" }, // HIGHRISK
            new() { Value = 999_999m,   ClientSector = "Public",  ClientId = "CLI004" }  // LOWRISK
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/trades/classify", trades);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ClassificationResponseDto>();
        Assert.NotNull(result);
        Assert.Equal(4, result!.Categories.Count);
        Assert.Equal(new[] { "LOWRISK", "MEDIUMRISK", "HIGHRISK", "LOWRISK" }, result.Categories);
    }

    [Fact]
    public async Task Classify_ReturnsBadRequest_WhenInvalidClientSectorIsSent()
    {
        // Arrange
        var trades = new List<TradeRequestDto>
        {
            new() { Value = 1_000_000m, ClientSector = "Corporate", ClientId = "CLI999" } // invalid sector
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/trades/classify", trades);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Invalid client sector", content);
    }

    [Fact]
    public async Task Analyze_ReturnsBadRequest_WhenEmptyListIsSent()
    {
        var emptyList = new List<TradeRequestDto>();

        var response = await _client.PostAsJsonAsync("/api/trades/analyze", emptyList);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Trade list cannot be empty", content);
    }

    [Fact]
    public async Task Analyze_ReturnsBadRequest_WhenMoreThan100000TradesAreSent()
    {
        // Arrange - simulate too many trades
        var tooManyTrades = Enumerable
            .Range(1, 100_001)
            .Select(i => new TradeRequestDto
            {
                Value = 500_000m,
                ClientSector = "Public",
                ClientId = $"CLI{i}"
            })
            .ToList();

        // Act
        var response = await _client.PostAsJsonAsync("/api/trades/analyze", tooManyTrades);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Maximum allowed trades per request is 100,000", content);
    }

    [Fact]
    public async Task Analyze_ReturnsCorrectSummary_ForSmallValidPortfolio()
    {
        // Arrange
        var trades = new List<TradeRequestDto>
        {
            // LOWRISK
            new() { Value = 900_000m,   ClientSector = "Private", ClientId = "CLI-A" },
            new() { Value = 400_000m,   ClientSector = "Public",  ClientId = "CLI-B" },

            // MEDIUMRISK
            new() { Value = 1_500_000m, ClientSector = "Public",  ClientId = "CLI-C" },
            new() { Value = 2_000_000m, ClientSector = "Public",  ClientId = "CLI-C" },

            // HIGHRISK
            new() { Value = 3_000_000m, ClientSector = "Private", ClientId = "CLI-D" }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/trades/analyze", trades);

        // Assert
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Dictionary<string, object>>(json)!;

        Assert.True(result.ContainsKey("categories"));
        Assert.True(result.ContainsKey("summary"));
        Assert.True(result.ContainsKey("processingTimeMs"));

        var categories = JsonSerializer.Deserialize<List<string>>(result["categories"]!.ToString()!)!;
        Assert.Equal(3, categories.Count);
        Assert.Equal(new[] { "LOWRISK", "MEDIUMRISK", "HIGHRISK" }, categories);

        var summaryJson = JsonSerializer.Serialize(result["summary"]);
        var summary = JsonSerializer.Deserialize<Dictionary<string, CategorySummaryDto>>(summaryJson, new JsonSerializerOptions(JsonSerializerDefaults.Web)!);

        Assert.Equal(3, summary.Count);

        // LOWRISK
        Assert.Equal(2, summary["LOWRISK"].Count);
        Assert.Equal(1_300_000m, summary["LOWRISK"].TotalValue);

        // MEDIUMRISK
        Assert.Equal(2, summary["MEDIUMRISK"].Count);
        Assert.Equal(3_500_000m, summary["MEDIUMRISK"].TotalValue);
        Assert.Equal("CLI-C", summary["MEDIUMRISK"].TopClient);

        // HIGHRISK
        Assert.Equal(1, summary["HIGHRISK"].Count);
        Assert.Equal(3_000_000m, summary["HIGHRISK"].TotalValue);
        Assert.Equal("CLI-D", summary["HIGHRISK"].TopClient);
    }

    [Fact]
    public async Task Analyze_ReturnsBadRequest_WhenClientIdIsMissingInAnyTrade()
    {
        var trades = new List<TradeRequestDto>
        {
            new() { Value = 1_000_000m, ClientSector = "Public", ClientId = "CLI001" },
            new() { Value = 2_000_000m, ClientSector = "Private", ClientId = null! }
        };

        var response = await _client.PostAsJsonAsync("/api/trades/analyze", trades);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        // Note: the exact message depends on where the validation happens (controller or analyzer)
    }
}