using Challenge_UBS.Domain.Entities;
using Challenge_UBS.Domain.Enums;
using Challenge_UBS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Challenge_UBS.Application
{
    //Class used to classify Low Risk trades
    public class LowRiskRule : IRiskRule
    {
        public RiskCategory Category => RiskCategory.LOWRISK;
        public bool IsMatch(Trade trade) => trade.Value < 1_000_000;
    }
}
