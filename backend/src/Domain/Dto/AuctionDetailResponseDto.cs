using Domain.Enum;

namespace Application.Dto.Auctions;

public sealed class AuctionDetailResponseDto
{
    public int Id { get; init; }
    public int VendedorId { get; init; }
    public int CategoriaId { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string Descripcion { get; init; } = string.Empty;
    public string UrlImagen { get; init; } = string.Empty;
    public decimal PrecioBase { get; init; }
    public decimal IncrementoMinimo { get; init; }
    public DateTime FechaInicio { get; init; }
    public DateTime FechaFin { get; init; }
    public EstadoSubasta Estado { get; init; }
    public byte[] Version { get; init; } = Array.Empty<byte>();
}
