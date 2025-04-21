namespace WebApplication1.Models
{
    public class ArchivoAdjunto
    {
        public int Id { get; set; }
        public int FormularioId { get; set; }
        public string NombreArchivo { get; set; }
        public string TipoArchivo { get; set; }
        public string RutaArchivo { get; set; }
        public DateTime FechaSubida { get; set; }
    }
}
