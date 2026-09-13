namespace Application.Interfaces.Persistencia.Lectura
{
    public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalItems);
}
