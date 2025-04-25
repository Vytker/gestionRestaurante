using System.Security.Claims;

namespace Shared.Extensions;

public static class HttpContextExtensions
{
    /// <summary>
    /// Obtiene el GUID del restaurante al que está vinculado el token JWT.
    /// Lanza excepción si el claim no existe o tiene formato incorrecto.
    /// </summary>
    public static Guid RestauranteId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("restauranteId")?.Value
                    ?? throw new UnauthorizedAccessException("Claim restauranteId ausente");

        return Guid.Parse(claim);   // Si no es un GUID válido lanzará FormatException
    }
}
