using System.ComponentModel.DataAnnotations;

namespace AplicacionNPG.Server.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? Apellido { get; set; }
        public string NombreUsuario { get; set; }
        [EmailAddress]
        public string? Correo { get; set; }
        public string Contrasena { get; set; }
        public int TipoUsuario { get; set; }
        public DateTime HoraRegistro { get; set; } = DateTime.Now;
        public DateTime HoraActualizacion { get; set; } = DateTime.Now;
        public int Estado { get; set; } = 1;
        public int RegistroId { get; set; }
        public int ActualizacionId { get; set; }
    }
}
