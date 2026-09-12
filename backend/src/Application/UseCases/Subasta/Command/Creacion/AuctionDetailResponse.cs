namespace Application.UseCases.Subasta.Command.Creacion
{
    public class AuctionDetailResponse
    {
        public int Id { get; set; }
        public int VendedorId { get; set; }
        public int CategoriaId { get; set; }
        public string Titulo { get; set; }= string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Url_imagen { get; set; }= string.Empty;
        public decimal Precio_base { get; set; }
        public decimal Incremento_minimo { get; set; }
        public DateTime Fecha_inicio { get; set; }
        public DateTime Fecha_fin { get; set; }
        public string Estado { get; set; }= string.Empty;
        public byte[] Version { get; set; } = Array.Empty<byte>();
    }
}
