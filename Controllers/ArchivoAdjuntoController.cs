using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DTO;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArchivoAdjuntoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ArchivoAdjuntoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ArchivoAdjunto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArchivoAdjunto>>> GetArchivoAdjunto()
        {
            return await _context.ArchivoAdjunto.ToListAsync();
        }

        // GET: api/ArchivoAdjunto/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ArchivoAdjunto>> GetArchivoAdjunto(int id)
        {
            var archivoAdjunto = await _context.ArchivoAdjunto.FindAsync(id);

            if (archivoAdjunto == null)
            {
                return NotFound();
            }

            return archivoAdjunto;
        }

        // PUT: api/ArchivoAdjunto/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArchivoAdjunto(int id, ArchivoAdjunto archivoAdjunto)
        {
            if (id != archivoAdjunto.Id)
            {
                return BadRequest();
            }

            _context.Entry(archivoAdjunto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArchivoAdjuntoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ArchivoAdjunto
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ArchivoAdjunto>> PostArchivoAdjunto(int formularioId, IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("Debe seleccionar un archivo válido.");

            var extensionesPermitidas = new[] { ".pdf", ".docx", ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(archivo.FileName).ToLower();

            if (!extensionesPermitidas.Contains(extension))
                return BadRequest("Tipo de archivo no permitido.");

            if (archivo.Length > 10 * 1024 * 1024)
                return BadRequest("Archivo demasiado grande. Máximo 10 MB.");

            var carpetaDestino = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(carpetaDestino))
                Directory.CreateDirectory(carpetaDestino);

            var nombreArchivo = Guid.NewGuid() + extension;
            var rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            var adjunto = new ArchivoAdjunto
            {
                FormularioId = formularioId,
                NombreArchivo = archivo.FileName,
                RutaArchivo = nombreArchivo,
                TipoArchivo = archivo.ContentType,
                FechaSubida = DateTime.Now
            };

            _context.Archivos.Add(adjunto);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Archivo subido correctamente", archivoId = adjunto.Id });
        
        }
        

        // DELETE: api/ArchivoAdjunto/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArchivoAdjunto(int id)
        {
            var archivoAdjunto = await _context.ArchivoAdjunto.FindAsync(id);
            if (archivoAdjunto == null)
            {
                return NotFound();
            }

            _context.ArchivoAdjunto.Remove(archivoAdjunto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArchivoAdjuntoExists(int id)
        {
            return _context.ArchivoAdjunto.Any(e => e.Id == id);
        }
    }
}
