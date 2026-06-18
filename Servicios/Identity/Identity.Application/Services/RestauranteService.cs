using Identity.Application.Dtos;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Infrastructure;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

            var owner = new User
            {
                UserName = dto.Owner.UserName,
                Email = dto.Owner.Email,
                FirstName = dto.Owner.FirstName,
                LastName = dto.Owner.LastName,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Owner.Password), // Método que implemente el hashing
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                Role = "Owner" // Se asigna directamente este rol
            };
            _context.Users.Add(owner);

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
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Email == dto.Email) ?? throw new Exception("No se ha encontrado el usuario");
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
            //comando para actualizar el usuario con el mismo id el rol de user a staff

            //'no poner el rol de staff en el usuario, ya que el rol se asigna en la tabla userrestaurantes'
            //user.Role = "Staff"; // No es necesario cambiar el rol del usuario, ya que se maneja en la tabla UserRestaurante
            //user.Role = "Staff";
           // _context.Users.Update(user);
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

        public async Task<IEnumerable<StaffDto>> ListarStaffAsync(Guid restauranteId)
        {
            return await _context.UserRestaurantes
                .Where(ur => ur.RestaurantId == restauranteId && ur.Role == "Staff")
                .Select(ur => new StaffDto(
                    ur.User.Id,
                    ur.User.UserName,
                    ur.User.Email,
                    $"{ur.User.FirstName} {ur.User.LastName}"
                ))
                .ToListAsync();
        }

        public async Task EliminarStaffAsync(Guid restauranteId, Guid staffId)
        {
            // Buscamos la asociación usuario-restaurante con rol “Staff”
            var staffAssociation = await _context.UserRestaurantes
                .FirstOrDefaultAsync(ur =>
                    ur.RestaurantId == restauranteId &&
                    ur.UserId == staffId &&
                    ur.Role == "Staff"
                );

            if (staffAssociation == null)
            {
                // Lanzamos una excepción específica para not found
                throw new KeyNotFoundException("El staff no fue encontrado para este restaurante.");
            }

            _context.UserRestaurantes.Remove(staffAssociation);
            var user = await _context.Users.FindAsync(staffId);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            else
            {
                // Si por alguna razón no existe el User (muy raro, porque
                // acabamos de encontrar la asociación), podríamos ignorar o lanzar:
                throw new KeyNotFoundException("El usuario asociado no se encontró en la tabla Users.");
            }

            await _context.SaveChangesAsync();
        }

    }
}
