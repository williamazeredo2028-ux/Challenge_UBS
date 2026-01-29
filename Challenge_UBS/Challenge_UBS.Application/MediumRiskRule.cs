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
    //Class used to classify Medium Risk trades
    public class MediumRiskRule : IRiskRule
    {
        public RiskCategory Category => RiskCategory.MEDIUMRISK;
        public bool IsMatch(Trade trade) =>
        trade.Value >= 1_000_000 && trade.ClientSector == "Public";
    }
}
