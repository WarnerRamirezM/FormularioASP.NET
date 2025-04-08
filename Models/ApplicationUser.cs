using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Column("nombre")]
        public string nombre { get; set; }
        public bool FormularioPendiente { get; set; }
    }
}
