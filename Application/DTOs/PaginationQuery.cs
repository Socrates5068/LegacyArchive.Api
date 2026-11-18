namespace LegacyArchive.Application.DTOs;

public class PaginationQuery
{
    public const int MaxPageSize = 100;

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
