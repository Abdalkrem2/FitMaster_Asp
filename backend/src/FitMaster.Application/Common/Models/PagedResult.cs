namespace FitMaster.Application.Common.Models;

public record PagedResult<T>(List<T> Content, int TotalElements, int TotalPages, int Number, int Size, bool First, bool Last)
{
    public static PagedResult<T> Create(List<T> pageItems, int totalElements, int page, int size)
    {
        var totalPages = size <= 0 ? 0 : (int)Math.Ceiling(totalElements / (double)size);
        return new PagedResult<T>(
            pageItems,
            totalElements,
            totalPages,
            page,
            size,
            First: page <= 0,
            Last: page >= totalPages - 1);
    }
}
