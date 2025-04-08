using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Pregunta
    {
        public int PreguntaId { get; set; }
        [Required]
        public string Texto { get; set; }
        [Required]
        public List<string> Opciones { get; set; } // Para preguntas de tipo "Selección"
       
    }
}
