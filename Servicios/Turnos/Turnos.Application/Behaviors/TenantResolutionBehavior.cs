using MediatR;
using Microsoft.AspNetCore.Http;
using Turnos.Application.Common;

namespace Turnos.Application.Behaviors
{
    /// Resuelve el RestaurantId (tenant) a partir del JWT o de la cabecera
    /// X-Restaurant-Id y lo inyecta en cualquier request que implemente ITenantScoped.

    public class TenantResolutionBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IHttpContextAccessor _http;

        public TenantResolutionBehavior(IHttpContextAccessor http) => _http = http;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (request is ITenantScoped scoped)
            {
                var ctx = _http.HttpContext!;
                var user = ctx.User;

                // 1) Owner → claim fijo
                var rawClaim = user.FindFirst("restauranteId")?.Value;
               var claimId = string.IsNullOrWhiteSpace(rawClaim) ? null : rawClaim;
                // 2) SuperAdmin → cabecera enviada desde el frontend
                var headerId = ctx.Request.Headers["X-Restaurante-Id"].FirstOrDefault();
                
                var idString = claimId ?? headerId
                    ?? throw new UnauthorizedAccessException("RestauranteId no especificado.");

                if (!Guid.TryParse(idString, out var restaurantId))
                    throw new UnauthorizedAccessException("RestaurantId inválido.");

                scoped.RestauranteId = Guid.Parse(idString);
            }

            return await next();
        }
    }
}
