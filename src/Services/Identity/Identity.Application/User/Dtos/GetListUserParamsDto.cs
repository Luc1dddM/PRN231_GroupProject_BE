using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.User.Dtos
{
    public class GetListUserParamsDto
    {
        public string? Keyword { get; set; }
        public DateOnly? Dob { get; set; }
        public string[]? Statuses { get; set; }
        public string[]? Genders { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string SortBy { get; set; } = "createdAt";
        public string SortOrder { get; set; } = "desc";
    }
}
