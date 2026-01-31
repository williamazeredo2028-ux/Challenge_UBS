# Challenge_UBS
.NET_Challenge_UBS

API REST desenvolvida em **ASP.NET Core 8** para classificar operações
financeiras por nível de risco.

O projeto segue princípios de **Clean Architecture** e **DDD
pragmático**, priorizando clareza, extensibilidade e performance.

## Funcionalidades

### Parte 1 --- Classificação de Risco

Classifica trades individualmente com base nas regras:

  Categoria    Regra
  ------------ ------------------------------------------------
  LOWRISK      Value \< 1.000.000
  
  MEDIUMRISK   Value ≥ 1.000.000 **e** ClientSector = Public
  
  HIGHRISK     Value ≥ 1.000.000 **e** ClientSector = Private

  **Entrada:** lista de operações
  
  **Saída:** lista de categorias de risco na mesma ordem
