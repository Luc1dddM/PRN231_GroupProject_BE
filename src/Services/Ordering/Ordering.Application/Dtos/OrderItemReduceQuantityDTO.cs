using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Dtos
{
    public record OrderItemReduceQuantityDTO(string productCproductCategoryId,
                                             int quantity,
                                             string user,
                                             bool IsCancel);
}
