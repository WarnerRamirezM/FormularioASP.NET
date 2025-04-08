namespace WebApplication1.Models
{
    public class People
    {
        //creamos una persona con un rol 
        public int PeopleId { get; set; }
        public string Name { get; set; }
        //Esta propiedad se va usar para la relacion con el formulario
        public int RoleId { get; set; }
        public Role Role { get; set; }
       
    }
}
