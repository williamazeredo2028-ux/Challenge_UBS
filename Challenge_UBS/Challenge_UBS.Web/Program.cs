using Challenge_UBS.Application.Services;
using Challenge_UBS.Domain.Rules;

var builder = WebApplication.CreateBuilder(args);

//// Add services to the container

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//// Dependency Injection

// Domain rules
builder.Services.AddScoped<IRiskRule, HighRiskRule>();
builder.Services.AddScoped<IRiskRule, MediumRiskRule>();
builder.Services.AddScoped<IRiskRule, LowRiskRule>();

// Application services
builder.Services.AddScoped<RiskClassifier>();
builder.Services.AddScoped<PortfolioAnalyzer>();

// Build application

var app = builder.Build();

// Configure HTTP request pipeline

if (app.Environment.IsDevelopment())
{
    // Enable Swagger only in Development
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enforce HTTPS
app.UseHttpsRedirection();

// Authorization middleware
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Run application
app.Run();