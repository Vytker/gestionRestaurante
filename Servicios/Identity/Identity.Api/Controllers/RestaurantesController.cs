using Identity.Application.Dtos;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/restaurantes")]
public class RestaurantesController : ControllerBase
{
    private readonly IRestauranteService _svc;
    public RestaurantesController(IRestauranteService svc) => _svc = svc;

    // Sólo SuperAdmin (p.e. un claim "sysadmin")
    // superadmin crea restaurante y asigna owner
    [Authorize(Roles = "SuperAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(RestauranteCreateDto dto)
    {
        var id = await _svc.CrearRestauranteAsync(dto);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    //superadmin obtiene listado global

    [Authorize(Roles = "SuperAdmin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _svc.ListarAsync();
        return Ok(result);
    }

    //superadmin o owner del restaurante ve el detalle
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        //superadmin pasa directo
        if(User.IsInRole("SuperAdmin"))
        {
            var result = await _svc.ObtenerAsync(id);
            return Ok(result);
        }

        //owner / staff: comprobar claim restuantId

        var claimRest = User.FindFirst("restaurantId")?.Value;
        if(claimRest == null || claimRest != id.ToString())
            return Forbid();

        return Ok(await _svc.ObtenerAsync(id));
    }


    // Owner delega Staff
    [Authorize(Roles = "Owner")]
    [HttpPost("{id:guid}/staff")]
    public async Task<IActionResult> AddStaff(Guid id, AddStaffDto dto)
    {
        //el owner solo puede actuar sobre su restaurante
        var claimRest = User.FindFirst("restaurantId")?.Value;
        if(claimRest == null || claimRest != id.ToString())
            return Forbid();

        await _svc.AsignarUsuarioAsync(id, dto);
        return Ok(new { message = "Usuario asignado correctamente" });


    }
}
