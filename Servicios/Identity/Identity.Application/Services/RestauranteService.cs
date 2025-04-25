using Identity.Application.Dtos;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Services
{
    public class RestauranteService : IRestauranteService
    { 
    
        private readonly IdentityDbContext _context;
        public RestauranteService(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CrearRestauranteAsync(RestauranteCreateDto dto)
        {
            var restaurante = new Restaurante
            {
                Nombre = dto.Nombre,
                Slug = dto.Slug,
            };
            _context.Restaurantes.Add(restaurante);

            //buscar el usuario que sera owner
            var owner = await _context.Users.FindAsync(dto.OwnerUserId) ?? throw new Exception("No se ha encontrado el usuario owner");

            owner.Role = "Owner"; // ascender a rol owner en user, no en la clase auxiliar userrestaurante, ya que leemos el rol de user abajo


            //insertar fila en userrestaurntes con rol owner
            _context.UserRestaurantes.Add(new UserRestaurante
            {
                UserId = owner.Id,
                RestaurantId = restaurante.Id,
                Role = "Owner"
            });
            
            await _context.SaveChangesAsync();
            return restaurante.Id;
        }
        public async Task AsignarUsuarioAsync(Guid restauranteId, AddStaffDto dto)
        {
            var rest = await _context.Restaurantes.FindAsync(restauranteId) ?? throw new Exception("No se ha encontrado el restaurante");
            //buscar el usuario que sera staff
            var user = await _context.Users.FindAsync(dto.UserId) ?? throw new Exception("No se ha encontrado el usuario");
            //insertar fila en userrestaurntes con rol staff
            if(await _context.UserRestaurantes.AnyAsync(ur => ur.UserId == user.Id && ur.RestaurantId == restauranteId))
            {
                throw new Exception("El usuario ya es staff de este restaurante");
            }

            var userRestaurante = new UserRestaurante
            {
                UserId = user.Id,
                RestaurantId = restauranteId,
                Role = "Staff"
            };
            _context.UserRestaurantes.Add(userRestaurante);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<RestaurantSummaryDto>> ListarAsync()
        {
            return await _context.Restaurantes
                .Select(r => new RestaurantSummaryDto(r.Id, r.Nombre, r.Slug))
                .ToListAsync();
        }
        public async Task<RestaurantSummaryDto?> ObtenerAsync(Guid id)
        {
            return await _context.Restaurantes
                .Where(r => r.Id == id)
                .Select(r => new RestaurantSummaryDto(r.Id, r.Nombre, r.Slug))
                .FirstOrDefaultAsync();
        }
    }
}
