# Challenge_UBS
.NET_Challenge_UBS

A REST API built with **ASP.NET Core 8** to classify financial trades 
by risk level and generate aggregated portfolio analysis.

The project follows **Clean Architecture** principles and **DDD approach** approach, 
focusing on clarity, extensibility, and performance.

## Features

### Part 1 --- Risk Classification

Classifies individual trades according to the rules below:

  Category    Rule
  ------------ ------------------------------------------------
  LOWRISK      Value \< 1.000.000
  
  MEDIUMRISK   Value ≥ 1.000.000 **and** ClientSector = Public
  
  HIGHRISK     Value ≥ 1.000.000 **and** ClientSector = Private


  **Input:** list of trades
  
  **Output:** list of risk categories in the same order

### Part 2 --- Portfolio Analysis

In addition to classification, it returns: - Number of trades per category - Total value added per category - Client with the highest exposure in each category - Request processing time

## Architecture

Domain → Application → Web → Tests

## How to Run

``` bash
dotnet restore
dotnet run --project Challenge_UBS.Web

```

Swagger available at `/swagger`.

## Tests

``` bash
dotnet test
```
