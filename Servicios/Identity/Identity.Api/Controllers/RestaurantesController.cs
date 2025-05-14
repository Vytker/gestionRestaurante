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
    [Authorize(Roles = "Owner,SuperAdmin")]
    [HttpPost("{id:guid}/staff")]
    public async Task<IActionResult> AddStaff(Guid id, AddStaffDto dto)
    {
        //el owner solo puede actuar sobre su restaurante
        if(!User.IsInRole("SuperAdmin"))
        {
           var claimRest = User.FindFirst("restauranteId")?.Value;
             if(claimRest == null || claimRest != id.ToString())
              {
                return Forbid(claimRest, id.ToString());
              }
        }
        
            

        await _svc.AsignarUsuarioAsync(id, dto);
        return Ok(new { message = "Usuario asignado correctamente" });


    }

    // RestaurantesController.cs
    [Authorize(Roles = "Owner,SuperAdmin")]
    [HttpGet("{id:guid}/staff/list")]
    public async Task<IActionResult> ListarStaff(Guid id)
    {
        //  Solo el Owner del restaurante puede ver su staff o cualquier superadmin
        if(!User.IsInRole("SuperAdmin"))
        {
            var claimRest = User.FindFirst("restauranteId")?.Value;
            if (!Guid.TryParse(claimRest, out var restId) || restId != id)
            return Forbid();
        }
        
        var staff = await _svc.ListarStaffAsync(id);
        return Ok(staff);
    }

}
