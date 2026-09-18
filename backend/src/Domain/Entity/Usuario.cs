using Domain.Common;
using Domain.Enum;

namespace Domain.Entity
{
    public class Usuario : IEntity
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public RolUsuario Rol { get; set; } = RolUsuario.Comprador;

        public DateTime FechaRegistro { get; set; }

        // Navegaciones
        public Billetera? Billetera { get; set; }

        public ICollection<Subasta> Subastas { get; set; } = new List<Subasta>();

        public ICollection<Puja> Pujas { get; set; } = new List<Puja>();

        public ICollection<AuditoriaLog> AuditoriaLogs { get; set; } = new List<AuditoriaLog>();
    }

}
