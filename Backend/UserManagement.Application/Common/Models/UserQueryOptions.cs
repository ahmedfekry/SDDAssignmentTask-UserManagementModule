namespace UserManagement.Application.Common.Models
{
    public class UserQueryOptions
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 8;
        public string? Search { get; set; }
        public string? Role { get; set; }
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
    }
}
