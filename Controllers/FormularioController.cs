using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DTO;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin, Instructor, Alumno")] // PARA LOS ROLES DEL AUTENTICACION
    public class FormularioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FormularioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Formulario
        [Authorize(Roles = "Admin, Alumno")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.formularios.ToListAsync());
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Formulario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FormularioId,TipoIdentificacion,Identificacion,Nombre,PrimerApellido,SegundoApellido,genero,Nacionalidad,FechaNacimiento,EstadoCivil,InstituciónEducativa,TelefonoPrincipal,TelefonoSecundario,fax,CorreoElectronico,CorreoElectronicoSecundario,Indigena,NombreCarrera,SeleccioneCarrera,NivelAcademico,TipoNecesidad,DescripcionNecesidad,CondicionMedica,RequiereAdaptaciones,AsistenciaAdicional,ApoyoAdicional,MedicoEspecialista,TratamientoTerapia,FechaDiagnostico")] Formulario formulario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(formulario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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
        public async Task<IActionResult> Edit(int id, [Bind("FormularioId,TipoIdentificacion,Identificacion,Nombre,PrimerApellido,SegundoApellido,genero,Nacionalidad,FechaNacimiento,EstadoCivil,InstituciónEducativa,TelefonoPrincipal,TelefonoSecundario,fax,CorreoElectronico,CorreoElectronicoSecundario,Indigena,NombreCarrera,SeleccioneCarrera,NivelAcademico,TipoNecesidad,DescripcionNecesidad,CondicionMedica,RequiereAdaptaciones,AsistenciaAdicional,ApoyoAdicional,MedicoEspecialista,TratamientoTerapia,FechaDiagnostico")] Formulario formulario)
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
