using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using WebApplication1.DTO;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Models.ApiModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin, Instructor, Alumno")] // PARA LOS ROLES DEL AUTENTICACION
    public class FormularioController : Controller
    {
        private readonly ApplicationDbContext _context; //clase db contexto
        private readonly EmailService _emailService; //clase service
        private readonly ApiMateriaServices _materiaService; //materias de la api
        private readonly ApiUniversidadServices _universidadServices;  //universidades de la api

        public FormularioController(ApplicationDbContext context, EmailService emailService, ApiMateriaServices materiaService, ApiUniversidadServices universidadServices) //inyectando contexto y el servicio de emails
        {
            _context = context;
            _emailService = emailService;
            _materiaService = materiaService; //inyectamos el servicio de api materias en el constructor
            _universidadServices = universidadServices;
        }

        // GET: Formulario
        [Authorize(Roles = "Admin, Alumno")]


        public async Task<IActionResult> Index(int page = 1) // Acción del controlador que muestra los formularios, con paginación. Por defecto, muestra la página 1.
        {
            int sizePage = 2; // Define cuántos formularios se muestran por página.

            try
            {
                // Cuenta cuántos formularios hay en total en la base de datos.
                int totalFormularios = await _context.formularios.CountAsync();

                // Calcula el total de páginas necesarias, redondeando hacia arriba si hay un residuo.
                var totalPaginas = (int)Math.Ceiling(totalFormularios / (double)sizePage);

                // Consulta los formularios para la página actual:
                // - Salta los formularios de las páginas anteriores.
                // - Toma solo los de la página actual.
                var formularios = await _context.formularios
                    .Skip((page - 1) * sizePage)    // Salta los registros de las páginas anteriores.
                    .Take(sizePage)                 // Toma solo los registros de la página actual.
                    .ToListAsync();                 // Ejecuta la consulta y obtiene los resultados como una lista.

                // Envía a la vista cuál es la página actual.
                ViewBag.PaginaActual = page;

                // Envía a la vista cuántas páginas hay en total.
                ViewBag.TotalPaginas = totalPaginas;

                // Retorna la vista con la lista de formularios para mostrarla.
                return View(formularios);
            }
            catch (System.Data.SqlTypes.SqlNullValueException ex) // Captura errores por valores NULL que no deberían serlo.
            {
                // Envía un mensaje de error a la vista si algún campo NULL causa una excepción.
                ViewBag.Error = "❌ Error: Un campo en la base de datos contiene un valor nulo inesperado.";

                // Imprime el error en la consola del servidor para depuración (útil en desarrollo).
                Console.WriteLine($"SqlNullValueException: {ex.Message}");

                // Retorna la vista con una lista vacía para evitar que se rompa.
                return View(new List<Formulario>());
            }
            catch (Exception ex) // Captura cualquier otro tipo de error que ocurra.
            {
                // Envía un mensaje genérico de error a la vista.
                ViewBag.Error = "❌ Ha ocurrido un error inesperado al cargar los formularios.";

                // Imprime el mensaje de error para que el desarrollador lo vea.
                Console.WriteLine($"Exception: {ex.Message}");

                // Devuelve una lista vacía a la vista para mantenerla funcional.
                return View(new List<Formulario>());
            }
        }


        // GET: Formulario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formulario = await _context.formularios
                .FirstOrDefaultAsync(m => m.FormularioId == id);
            if (formulario == null)
            {
                return NotFound();
            }

            return View(formulario);
        }

        // GET: Formulario/Create
        public async Task<IActionResult> CreateAsync()
        {
            var universidades = await _universidadServices.ObtenerUniversidadesAsync();

            // Convertir a SelectListItem
            ViewBag.Universidad = universidades.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(), // Id debe ser int o string
                Text = u.Name
            }).ToList();

            //obtener las materias del servicio
            var materias = await _materiaService.ObtenerMateriasAsync();

            ViewBag.Materia = materias.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.Name
            }).ToList();

            return View();
        }

        // POST: Formulario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FormularioId,TipoIdentificacion,Identificacion,Nombre,PrimerApellido,SegundoApellido,genero,Nacionalidad,FechaNacimiento,EstadoCivil,TelefonoPrincipal,TelefonoSecundario,fax,CorreoElectronico,CorreoElectronicoSecundario,Indigena,NombreCarrera,NivelAcademico,TipoNecesidad,DescripcionNecesidad,CondicionMedica,RequiereAdaptaciones,AsistenciaAdicional,ApoyoAdicional,MedicoEspecialista,TratamientoTerapia,FechaDiagnostico, UniversidadId, MateriaId")] Formulario formulario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(formulario);
                await _context.SaveChangesAsync();

                // ✅ Enviar correo
                var mensaje = $@"
            <h2>Formulario creado con éxito</h2>
            <p>Estimado/a {formulario.Nombre} {formulario.PrimerApellido},</p>
            <p>Su formulario ha sido registrado correctamente en el sistema.</p>
            <p><strong>Número de formulario:</strong> {formulario.FormularioId}</p>
            <p><strong>Identificación:</strong> {formulario.Identificacion}</p>
            <p><strong>Correo registrado:</strong> {formulario.CorreoElectronico}</p>
            <br/>
            <p>Gracias por utilizar el sistema de la UNED.</p>
        ";

                await _emailService.EnviarCorreoAsync(
                    destinatario: formulario.CorreoElectronico,
                    asunto: "Formulario creado con éxito - UNED",
                    mensaje: mensaje
                );

                return RedirectToAction(nameof(Index));
            }
            //CARGAR LOS DATOS DE UNIVERSIDAD Y MATERIAS
            //obtener las universidades del servicio
            var universidades = await _universidadServices.ObtenerUniversidadesAsync();
            ViewBag.Universidad = universidades; //mandamos el viewbag de universidades disponibles.

            //obtener las materias del servicio
            var materias = await _materiaService.ObtenerMateriasAsync();

            // Puedes pasarlas a la vista con ViewBag, ViewModel, etc.
            ViewBag.Materia = materias;
            
            return View(formulario);
        }


        // GET: Formulario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formulario = await _context.formularios.FindAsync(id);
            if (formulario == null)
            {
                return NotFound();
            }
            return View(formulario);
        }

        // POST: Formulario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FormularioId,TipoIdentificacion,Identificacion,Nombre,PrimerApellido,SegundoApellido,genero,Nacionalidad,FechaNacimiento,EstadoCivil,TelefonoPrincipal,TelefonoSecundario,fax,CorreoElectronico,CorreoElectronicoSecundario,Indigena,NombreCarrera,SeleccioneCarrera,NivelAcademico,TipoNecesidad,DescripcionNecesidad,CondicionMedica,RequiereAdaptaciones,AsistenciaAdicional,ApoyoAdicional,MedicoEspecialista,TratamientoTerapia,FechaDiagnostico")] Formulario formulario)
        {
            if (id != formulario.FormularioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(formulario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormularioExists(formulario.FormularioId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(formulario);
        }

        // GET: Formulario/Delete/5
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdministradorPolicy")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formulario = await _context.formularios
                .FirstOrDefaultAsync(m => m.FormularioId == id);
            if (formulario == null)
            {
                return NotFound();
            }

            return View(formulario);
        }

        // POST: Formulario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var formulario = await _context.formularios.FindAsync(id);
            if (formulario != null)
            {
                _context.formularios.Remove(formulario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FormularioExists(int id)
        {
            return _context.formularios.Any(e => e.FormularioId == id);
        }
        public IActionResult DescargarPDF(int id)
        {
            var formulario = _context.formularios
                .Where(f => f.FormularioId == id)
                .FirstOrDefault();

            if (formulario == null)
            {
                return NotFound();
            }

            using (MemoryStream ms = new MemoryStream())
            {
                using (Document doc = new Document())
                {
                    PdfWriter.GetInstance(doc, ms);
                    doc.Open();
                    doc.Add(new Paragraph("Gestión de formularios para usuarios y estudiantes de la Universidad Estatal a Distancia UNED"));
                    doc.Add(new Chunk("\n"));
                    doc.Add(new Paragraph("Identificación: " + formulario.Identificacion));
                    doc.Add(new Paragraph("Nombre: " + formulario.Nombre));
                    doc.Add(new Paragraph($"Primer Apellido: {formulario.PrimerApellido}"));
                    doc.Add(new Paragraph($"Segundo Apellido: {formulario.SegundoApellido}"));
                    doc.Add(new Paragraph($"Nacionalidad: {formulario.Nacionalidad}"));
                    doc.Add(new Paragraph($"Teléfono Principal: {formulario.TelefonoPrincipal}"));
                    doc.Add(new Paragraph($"Correo Electrónico: {formulario.CorreoElectronico}"));
                    doc.Add(new Paragraph($"Nombre Carrera: {formulario.NombreCarrera}"));
                    // Agrega más campos según sea necesario

                    doc.Close();
                }
                return File(ms.ToArray(), "application/pdf", "Formulario_" + formulario.FormularioId + ".pdf");
            }
        }
    }
}
