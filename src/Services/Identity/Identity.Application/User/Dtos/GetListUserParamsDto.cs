namespace Identity.Application.User.Dtos
{
    public class GetListUserParamsDto
    {
        public string? Keyword { get; set; }
        public DateOnly? BirthDayFrom { get; set; }
        public DateOnly? BirthDayTo { get; set; }
        public string[]? Statuses { get; set; }
        public string[]? Genders { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string SortBy { get; set; } = "Id";
        public string SortOrder { get; set; } = "desc";
    }
}
