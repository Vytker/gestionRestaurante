

namespace Identity.Application.Dtos
{
    public class OwnerCreateDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        // Otros campos que consideres necesarios
    }
    public class RestauranteCreateDto
    {
        public string Nombre { get; set; } = "";
        public string Slug { get; set; } = "";   // ej. "madrid-centro"
        public OwnerCreateDto Owner { get; set; } = new OwnerCreateDto();
    }
}
