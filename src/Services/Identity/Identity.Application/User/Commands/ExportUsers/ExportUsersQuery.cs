using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Identity.Application.User.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.User.Commands.ExportUsers
{
    public record ExportUsersQuery(ExportListParamsDto parameters) : IQuery<ExportUsersResponse>;

    public record ExportUsersResponse(BaseResponse<DataTable> response);
}
