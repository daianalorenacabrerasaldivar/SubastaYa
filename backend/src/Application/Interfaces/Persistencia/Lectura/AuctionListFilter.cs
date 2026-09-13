namespace Application.Interfaces.Persistencia.Lectura
{
    public sealed record AuctionListFilter(
        AuctionStatusFilter? Status,
        int? CategoryId,
        decimal? MinPrice,
        decimal? MaxPrice,
        AuctionSortBy SortBy,
        int Page,
        int PageSize);
}
