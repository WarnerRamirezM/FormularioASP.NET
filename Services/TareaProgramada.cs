using Microsoft.EntityFrameworkCore;
using WebApplication1.DTO;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class TareaProgramada
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;
        private readonly ILogger<TareaProgramada> _logger;

        // Constructor donde se inyectan las dependencias
        public TareaProgramada(ApplicationDbContext context, EmailService emailService, ILogger<TareaProgramada> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        // Método principal para enviar las notificaciones
        public async Task EnviarNotificaciones()
        {
            try
            {
                // Obtener usuarios pendientes de completar el formulario
                var usuariosPendientes = await _context.Users
                                                        .Where(u => u.FormularioPendiente)
                                                        .ToListAsync();  // Usamos ToListAsync para hacer la consulta asincrónica

                foreach (var usuario in usuariosPendientes)
                {
                    string asunto = "Recordatorio: Completa tu formulario";
                    string mensaje = GenerarMensajeRecordatorio(usuario);

                    try
                    {
                        // Ejecutar el envío de correo asincrónicamente
                        await _emailService.EnviarCorreoAsync(usuario.Email, asunto, mensaje);
                        _logger.LogInformation($"Correo enviado exitosamente a {usuario.Email}");
                    }
                    catch (Exception ex)
                    {
                        // Log de error si algo sale mal
                        _logger.LogError($"Error al enviar correo a {usuario.Email}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log de error si la consulta o el proceso general falla
                _logger.LogError($"Error al obtener usuarios pendientes o procesar notificaciones: {ex.Message}");
            }
        }

        // Método para generar el mensaje personalizado
        private string GenerarMensajeRecordatorio(ApplicationUser usuario)
        {
            return $"Hola {usuario.UserName},<br><br>Recuerda que tienes un formulario pendiente por completar.";
        }
    }
}
