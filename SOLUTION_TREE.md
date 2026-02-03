Challenge_UBS.sln
│
├── Challenge_UBS.Domain                # Core business logic and domain rules
│   │
│   ├── Enums                           # Domain enumerations
│   │   ├── ClientSector.cs             # Defines client sectors (Public, Private)
│   │   └── RiskCategory.cs             # Defines risk categories (Low, Medium, High)
│   │
│   ├── Models                          # Domain entities
│   │   └── Trade.cs                    # Represents a financial trade
│   │
│   ├── Rules                           # Risk classification rules
│   │   ├── IRiskRule.cs                # Contract for all risk rules
│   │   ├── HighRiskRule.cs             # Rule for high-risk trades
│   │   ├── MediumRiskRule.cs           # Rule for medium-risk trades
│   │   └── LowRiskRule.cs              # Rule for low-risk trades
│   │
│   └── Challenge_UBS.Domain.csproj     # Domain project configuration
│
├── Challenge_UBS.Application           # Application and orchestration layer
│   │
│   ├── DTOs                            # Data Transfer Objects (API contracts)
│   │   ├── TradeRequestDto.cs          # Input DTO for trade requests
│   │   ├── ClassificationResponseDto.cs# Output DTO for trade classification
│   │   ├── CategorySummaryDto.cs       # Aggregated data per risk category
│   │   └── PortfolioSummaryDto.cs      # Portfolio analysis response DTO
│   │
│   ├── Models                          # Application models
│   │   ├── CategorySummary.cs          # Accumulator for category statistics
│   │   └── PortfolioSummary.cs         # Final portfolio analysis result
│   │
│   ├── Services                        # Application services
│   │   ├── RiskClassifier.cs           # Applies risk rules to trades
│   │   └── PortfolioAnalyzer.cs        # Aggregates portfolio risk statistics
│   │
│   └── Challenge_UBS.Application.csproj# Application project configuration
│
├── Challenge_UBS.Web                   # Web/API layer
│   │
│   ├── Controllers                     # API controllers
│   │   └── TradesController.cs         # Exposes trade classification endpoints
│   │
│   ├── Program.cs                      # Application startup and DI configuration
│   └── Challenge_UBS.Web.csproj        # Web project configuration
│
├── Challenge_UBS.Tests                 # Test project
│   │
│   ├── Domain                          # Unit tests for domain rules
│   │   ├── HighRiskRuleTests.cs
│   │   ├── MediumRiskRuleTests.cs
│   │   └── LowRiskRuleTests.cs
│   │
│   ├── Application                     # Unit tests for application services
│   │   ├── RiskClassifierTests.cs
│   │   └── PortfolioAnalyzerTests.cs
│   │
│   ├── Web                             # Integration tests for API endpoints
│   │   └── TradesControllerIntegrationTests.cs
│   │
│   └── Challenge_UBS.Tests.csproj      # Test project configuration
│
├── README_PT.md                        # Project documentation in portuguese
└── README_EN.md                        # Project documentation in english
