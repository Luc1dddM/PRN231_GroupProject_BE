namespace Email.API.DTOs;

public class GetListEmailParamsDto
{
    public string[] Statuses { get; set; } = Array.Empty<string>();
    public string[] Categories { get; set; } = Array.Empty<string>();
    public string SearchTerm { get; set; } = string.Empty;
    public string SortBy { get; set; } = "createdDate"; 
    public string SortOrder { get; set; } = "asc";     
    public int PageNumber { get; set; } = 1;           
    public int PageSize { get; set; } = 10;           
}