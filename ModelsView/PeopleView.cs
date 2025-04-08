using WebApplication1.Models;

namespace WebApplication1.ModelsView
{
    public class PeopleView: People
    {
        // Propiedades del Formulario que quieres incluir en la vista
        public Formulario Formulario { get; set; }
        public string FormularioName { get; set; }
        public string FormularioDescription { get; set; }
        public string FormularioEmail { get; set; }
    }
}
