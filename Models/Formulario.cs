namespace WebApplication1.Models
{
    public class Formulario
    {
        public int FormularioId { get; set; }
        public string TipoIdentificacion {  get; set; }
        public string Identificacion {  get; set; }
        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }  // Apellido completo del estudiante
        public string SegundoApellido { get; set; }
        public string genero { get; set; }
        public string Nacionalidad { get; set; }  // País de residencia
        public DateTime FechaNacimiento { get; set; }  // Fecha de nacimiento del estudiante
        public string EstadoCivil { get; set; }  // Estado civil del estudiante
        public string InstituciónEducativa { get; set; }  // Institución educativa de procedencia
        
        public string TelefonoPrincipal { get; set; }  // Teléfono de contacto
        public string TelefonoSecundario { get; set; }
        public string fax { get; set; }
        public string CorreoElectronico { get; set; }  // Correo electrónico
        public string CorreoElectronicoSecundario { get; set; }
        public Boolean Indigena { get; set; }
        public string NombreCarrera { get; set; }
        public string SeleccioneCarrera { get; set; }
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

    }
}
