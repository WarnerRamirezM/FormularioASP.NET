using Newtonsoft.Json;
using WebApplication1.Models.ApiModels;

namespace WebApplication1.Services
{
    public class ApiUniversidadServices
    {
        private readonly HttpClient _client;

        // Constructor con HttpClient inyectado
        public ApiUniversidadServices(HttpClient client)
        {
            _client = client;
        }
        public async Task<List<UniversidadModel>> ObtenerUniversidadesAsync()
        {
            // Realiza una petición GET al endpoint del API para obtener las universidades
            var response = await _client.GetAsync("api/Universidads");

            // Verifica si la respuesta fue exitosa (código 200 OK)
            if (response.IsSuccessStatusCode)
            {
                // Lee el contenido de la respuesta como un string JSON
                var json = await response.Content.ReadAsStringAsync();

                // Deserializa el JSON a una lista de objetos MateriaModel
                var universidades = JsonConvert.DeserializeObject<List<UniversidadModel>>(json);

                // Verifica si la deserialización no devolvió null
                if (universidades != null)
                    return universidades; // Retorna la lista deserializada
                else
                    return new List<UniversidadModel>(); // Si fue null, retorna una lista vacía
            }

            // Si la respuesta no fue exitosa, retorna una lista vacía
            return new List<UniversidadModel>();
        }

    }

}
