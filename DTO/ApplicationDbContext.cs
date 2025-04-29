using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.DTO
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> // Usamos IdentityDbContext con ApplicationUser
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        public DbSet<Formulario> formularios { get; set; }
        public DbSet<People> People { get; set; }
        public DbSet<Pregunta> preguntas { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<ArchivoAdjunto> Archivos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Aquí puedes agregar configuraciones adicionales si es necesario
            // Configurar la relación entre Formulario y Person

         
        }
        public DbSet<WebApplication1.Models.ArchivoAdjunto> ArchivoAdjunto { get; set; } = default!;
    }
}
