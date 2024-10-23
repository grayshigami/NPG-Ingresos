namespace AplicacionNPG.Server.Models
{
    public class Incidencia
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? Empresa { get; set; }
        public DateTime HoraEntrada { get; set; }
        public DateTime? HoraSalida { get; set; }
        public string? Comentario { get; set; }
        public int UsuarioId { get; set; }
        public int? ActualizacionId { get; set; }
    }
}
