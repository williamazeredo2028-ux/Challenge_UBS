using Challenge_UBS.Application;
using Challenge_UBS.Application.Services;
using Challenge_UBS.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//AddScoped to register risk rules
builder.Services.AddScoped<IRiskRule, LowRiskRule>();
builder.Services.AddScoped<IRiskRule, MediumRiskRule>();
builder.Services.AddScoped<IRiskRule, HighRiskRule>();

builder.Services.AddScoped<RiskClassifier>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
