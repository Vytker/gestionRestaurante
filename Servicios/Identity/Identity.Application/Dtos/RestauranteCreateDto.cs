

namespace Identity.Application.Dtos
{
    public class RestauranteCreateDto
    {
        public string Nombre { get; set; } = "";
        public string Slug { get; set; } = "";   // ej. "madrid-centro"
        public Guid OwnerUserId { get; set; }
    }
}
