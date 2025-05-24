namespace Turnos.Application.Common
{

    /// Marca todos los comandos / queries que necesitan conocer el restaurante
    /// (tenant) sobre el que opera la petición.

    public interface ITenantScoped
    {
        
        /// El identificador del restaurante sobre el que se ejecutará el handler.
        /// El TenantResolutionBehavior lo rellenará antes de que el handler se dispare.
        
        Guid RestauranteId { get; set; }
    }
}
