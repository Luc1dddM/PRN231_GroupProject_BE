using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Dtos
{
    public class StatisticDTO
    {
        public List<decimal> StatisticIncomeForYear { get; set; } = default!;
        public List<decimal> StatisticImcomeForFourWeek { get; set; } = default!;
        public decimal StatisticImcomePerDay { get; set; }
        public int StatisticProductSaledPerDay {  get; set; }
    }
}
