

namespace Identity.Domain.Entities
{
    public class Restaurante
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nombre { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;  // para la URL
        public ICollection<UserRestaurante> Usuarios { get; set; } = new List<UserRestaurante>();
    }
}
