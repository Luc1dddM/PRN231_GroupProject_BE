using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Orders.Queries.Statistic
{
    public record StatisticQuery() : IQuery<StatisticResult>;

    public record StatisticResult(BaseResponse<StatisticDTO> Result);
}
