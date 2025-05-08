using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Models.ApiModels;

namespace WebApplication1.Models
{
    public class Formulario
    {
        
        public int FormularioId { get; set; }
        public string TipoIdentificacion {  get; set; }
        public string Identificacion {  get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }  // Apellido completo del estudiante
        public string SegundoApellido { get; set; }
        public string Genero { get; set; }
        public string Nacionalidad { get; set; }  // País de residencia
        public DateTime FechaNacimiento { get; set; }  // Fecha de nacimiento del estudiante
        public string EstadoCivil { get; set; }  // Estado civil del estudiante
        
        public string TelefonoPrincipal { get; set; }  // Teléfono de contacto
        public string TelefonoSecundario { get; set; }
        public string Fax { get; set; }
        public string CorreoElectronico { get; set; }  // Correo electrónico
        public string CorreoElectronicoSecundario { get; set; }
        public Boolean Indigena { get; set; }
        public string NombreCarrera { get; set; }
        public string NivelAcademico { get; set; }
        public string TipoNecesidad { get; set; }
        public string DescripcionNecesidad { get; set; }
        public Boolean CondicionMedica { get; set; }
        public Boolean RequiereAdaptaciones { get; set; }
        public Boolean AsistenciaAdicional { get; set; }
        public string ApoyoAdicional { get; set; }
        public string MedicoEspecialista { get; set; }
        public string TratamientoTerapia { get; set; }
        public DateTime FechaDiagnostico { get; set; }
        // Aquí añadimos la propiedad para almacenar los archivos
        [NotMapped] // <- Esto evita que EF intente mapearlo en la DB
        [BindNever]
        public IFormFile ArchivoSubido { get; set; }  // Para recibir el archivo desde la vista
        [BindNever]
        public String? FilePath { get; set; } // Guarda la ruta del archivo
        [Required(ErrorMessage = "Debe seleccionar una universidad")]
        public int UniversidadId { get; set; }
        public int MateriaId { get; set; }
       
    }
}
