namespace Application.Common
{
    public sealed record PagedResponse<T>(
        IReadOnlyList<T> Items,
        int Page,
        int PageSize,
        int TotalItems,
        int TotalPages,
        DateTime FechaServidor)
    {
        public static PagedResponse<T> Create(IReadOnlyList<T> items, int page, int pageSize, int totalItems, DateTime fechaServidor)
        {
            var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);
            return new PagedResponse<T>(items, page, pageSize, totalItems, totalPages, fechaServidor);
        }
    }
}
