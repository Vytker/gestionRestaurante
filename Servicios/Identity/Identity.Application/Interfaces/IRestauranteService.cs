

using Identity.Application.Dtos;

namespace Identity.Application.Interfaces
{
    public interface IRestauranteService
    {
        Task<Guid> CrearRestauranteAsync(RestauranteCreateDto dto); // devuelve un id
        Task<IEnumerable<RestaurantSummaryDto>> ListarAsync(); // devuelve una lista de restaurantes
        Task<RestaurantSummaryDto?> ObtenerAsync(Guid id); // devuelve un restaurante por id
        Task AsignarUsuarioAsync(Guid restauranteId, AddStaffDto dto); // asigna un usuario a un restaurante owner -> staff

    }
}
