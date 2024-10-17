using Ordering.Application.Orders.Queries.GetOrders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Orders.Queries.Statistic
{
    public class StatisticHandler : IQueryHandler<StatisticQuery, StatisticResult>
    {
        private readonly IApplicationDbContext _context;

        public StatisticHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<StatisticResult> Handle(StatisticQuery query, CancellationToken cancellationToken)
        {
            var statisticImcomePerDay = await StatisticImcomePerDay();
            var statisticProductSaledPerDay = await StatisticProductSaledPerDay();
            var statisticIncomeForYear = await StatisticIncomeForYear();
            var statisticImcomeForFourWeek = await StatisticImcomeForFourWeek();

            var result = new StatisticDTO
            {
                StatisticImcomeForFourWeek = statisticImcomeForFourWeek,
                StatisticImcomePerDay = statisticImcomePerDay,
                StatisticIncomeForYear = statisticIncomeForYear,
                StatisticProductSaledPerDay = statisticProductSaledPerDay
            };

            return new StatisticResult(new BaseResponse<StatisticDTO>(result));

        }


        public async Task<decimal> StatisticImcomePerDay()
        {
            try
            {
                return  _context.Orders.Where(o => o.LastModified.HasValue && o.LastModified.Value.Date == DateTime.Now.Date && o.Status.Equals("Shipped")).Sum(o => o.TotalPrice);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<int> StatisticProductSaledPerDay()
        {
            try
            {
                var dayOrder = _context.Orders.Include(o => o.OrderItems).Where(o => o.LastModified.HasValue && o.LastModified.Value.Date == DateTime.Now.Date && o.Status.Equals("Shipped"));
                int productSaled = 0;
                foreach (var item in dayOrder)
                {
                    productSaled += item.OrderItems.Sum(o => o.Quantity);
                }
                return productSaled;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<decimal>> StatisticIncomeForYear()
        {
            try
            {
                List<decimal> list = new List<decimal>();
                for (var i = 1; i < 13; i++)
                {
                    var orders = _context.Orders.Where(c => c.LastModified.HasValue && c.LastModified.Value.Month == i && c.Status.Equals("Shipped"));
                    decimal income = 0;
                    foreach (var order in orders)
                    {
                        income += order.TotalPrice;
                    }
                    list.Add(income);
                }
                return list;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<decimal>> StatisticImcomeForFourWeek()
        {
            try
            {
                List<decimal> list = new List<decimal>();
                var firstDayOfCurrentWeek = GetTheFirstDateOfWeek();
                for (var i = 0; i < 4; i++)
                {
                    var orders = _context.Orders.Where(
                        c => c.LastModified.HasValue &&
                        c.LastModified.Value.Date >= firstDayOfCurrentWeek.AddDays(-i * 7) &&
                        c.LastModified.Value.Date <= firstDayOfCurrentWeek.AddDays((-i * 7) + 6)
                        && c.Status.Equals("Shipped"));
                    decimal income = 0;
                    foreach (var order in orders)
                    {
                        income += order.TotalPrice;
                    }
                    list.Add(income);
                }
                return list;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }



        private DateTime GetTheFirstDateOfWeek()
        {
            try
            {
                DateTime d = DateTime.Now;
                var culture = CultureInfo.CurrentCulture;
                var diff = d.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
                if (diff < 0)
                    diff += 7;
                d = d.AddDays(-diff).Date;
                return d;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
