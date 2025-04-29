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
        public async Task<ActionResult<ArchivoAdjunto>> PostArchivoAdjunto(int formularioId, IFormFile archivo) // Método para subir un archivo adjunto y asociarlo a un formulario.
        {
            if (archivo == null || archivo.Length == 0) // Si el archivo es nulo o está vacío (sin contenido), retorna error.
                return BadRequest("Debe seleccionar un archivo válido."); // Devuelve una respuesta HTTP 400 con un mensaje.

            var extensionesPermitidas = new[] { ".pdf", ".docx", ".jpg", ".jpeg", ".png" }; // Arreglo con las extensiones de archivo permitidas.

            var extension = Path.GetExtension(archivo.FileName).ToLower(); // Obtiene la extensión del archivo cargado y la convierte a minúscula.

            if (!extensionesPermitidas.Contains(extension)) // Verifica si la extensión del archivo no está en la lista permitida.
                return BadRequest("Tipo de archivo no permitido."); // Retorna error si la extensión no es válida.

            if (archivo.Length > 10 * 1024 * 1024) // Verifica si el archivo pesa más de 10 MB.
                return BadRequest("Archivo demasiado grande. Máximo 10 MB."); // Retorna error si excede el límite.

            var carpetaDestino = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads"); // Define la ruta de la carpeta donde se guardará el archivo.

            if (!Directory.Exists(carpetaDestino)) // Verifica si la carpeta no existe.
                Directory.CreateDirectory(carpetaDestino); // Si no existe, la crea.

            var nombreArchivo = Guid.NewGuid() + extension; // Genera un nombre único para el archivo usando un GUID y le agrega la extensión original.

            var rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo); // Construye la ruta completa donde se almacenará el archivo.

            using (var stream = new FileStream(rutaCompleta, FileMode.Create)) // Crea un flujo de archivo para escribir el contenido del archivo subido.
            {
                await archivo.CopyToAsync(stream); // Copia el contenido del archivo subido al archivo físico en el servidor.
            }

            var adjunto = new ArchivoAdjunto // Crea una nueva instancia del modelo ArchivoAdjunto para guardar la información del archivo en la base de datos.
            {
                FormularioId = formularioId, // Asocia el archivo al formulario con el ID proporcionado.
                NombreArchivo = archivo.FileName, // Guarda el nombre original del archivo.
                RutaArchivo = nombreArchivo, // Guarda el nombre con el que se almacenó el archivo (con GUID).
                TipoArchivo = archivo.ContentType, // Guarda el tipo MIME del archivo (por ejemplo, image/png, application/pdf).
                FechaSubida = DateTime.Now // Guarda la fecha y hora actual como momento de subida.
            };

            _context.Archivos.Add(adjunto); // Agrega el nuevo objeto adjunto al contexto para que sea guardado en la base de datos.
            await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos de forma asíncrona.

            return Ok(new { mensaje = "Archivo subido correctamente", archivoId = adjunto.Id }); // Retorna una respuesta HTTP 200 con un mensaje y el ID del archivo subido.
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
