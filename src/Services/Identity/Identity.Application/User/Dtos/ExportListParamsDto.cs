using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.User.Dtos
{
    public class ExportListParamsDto
    {
        public string? Keyword { get; set; }
        public DateOnly? BirthDayFrom { get; set; }
        public DateOnly? BirthDayTo { get; set; }
        public string[]? Statuses { get; set; }
        public string[]? Genders { get; set; }
        public string? SortBy { get; set; } = "Id";
        public string? SortOrder { get; set; } = "desc";
    }
}
