using Domain.Common.ResultPattern;
using MediatR;

namespace Application.UseCases.Subasta.Command.Creacion
{
    public class CreateAuctionCommand : IRequest<Result<AuctionDetailResponse>>
    {
        public int VendedorId { get; set; }
        public int CategoriaId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string UrlImagen { get; set; } = string.Empty;
        public decimal PrecioBase { get; set; }
        public decimal IncrementoMinimo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
