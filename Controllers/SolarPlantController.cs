using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UpriseMidTask.Data;
using UpriseMidTask.Models;

namespace UpriseMidTask.Controllers;

[Authorize]
[ApiController]
[Route("solar-plants")]
public class SolarPlantController : ControllerBase
{
    private readonly ILogger<SolarPlantController> _logger;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;

    public SolarPlantController(IConfiguration configuration, AppDbContext context, ILogger<SolarPlantController> logger)
    {
        _configuration = configuration;
        _context = context;
        _logger = logger;
    }

    [HttpPost("create")] 
    public async Task<ActionResult<SolarPlant>> CreateSolarPowerPlant(SolarPlant plant)
    {
        _context.SolarPlants.Add(plant);
        await _context.SaveChangesAsync();
        return CreatedAtAction("GetSolarPlant", new { id = plant.Id }, plant);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SolarPlant>> GetSolarPlant(int id)
    {
        var plant = await _context.SolarPlants.FindAsync(id);
        if (plant == null)
        {
            return NotFound();
        }
        return Ok(plant);
    }
}
