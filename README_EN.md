# Challenge_UBS
.NET_Challenge_UBS

A REST API built with **ASP.NET Core 8** to classify financial trades 
by risk level and generate aggregated portfolio analysis.

The project follows **Clean Architecture** principles and **DDD approach** approach, 
focusing on clarity, extensibility, and performance.

## Features

### Part 1 — Risk Classification

Classifies individual trades according to the rules below:

  Categoria    Regra
  ------------ ------------------------------------------------
  LOWRISK      Value \< 1.000.000
  
  MEDIUMRISK   Value ≥ 1.000.000 **and** ClientSector = Public
  
  HIGHRISK     Value ≥ 1.000.000 **and** ClientSector = Private

  **Input:** list of trades
  **Output:** list of risk categories in the same order
