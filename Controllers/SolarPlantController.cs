using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        var plant = await _context.SolarPlants
        .Include(p => p.ProductionData)
        .FirstOrDefaultAsync(p => p.Id == id);

        if (plant == null)
        {
            return NotFound();
        }
        return Ok(plant);
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<IEnumerable<SolarPlant>>> GetAllSolarPlants()
    {
        var plants = await _context.SolarPlants
        .Where(p => p.DeletedAt == null)
        .Include(p => p.ProductionData)
        .ToListAsync();

        if (plants == null || plants.Count == 0)
        {
            return NotFound("No solar plants found.");
        }

        return Ok(plants);
    }

    [HttpPatch("{id}/update")]
    public async Task<ActionResult<SolarPlant>> UpdateSolarPlant(int id, [FromBody] SolarPlant updatedPlant)
    {
        var plant = await _context.SolarPlants.FindAsync(id);
        if (plant == null)
        {
            return NotFound();
        }

        if (updatedPlant.Name != null) plant.Name = updatedPlant.Name;
        if (updatedPlant.PowerInstalled != default(decimal)) plant.PowerInstalled = updatedPlant.PowerInstalled;
        if (updatedPlant.DateInstalled != default(DateTime)) plant.DateInstalled = updatedPlant.DateInstalled;
        if (updatedPlant.Latitude != default(double)) plant.Latitude = updatedPlant.Latitude;
        if (updatedPlant.Longitude != default(double)) plant.Longitude = updatedPlant.Longitude;

        await _context.SaveChangesAsync();
        return Ok(updatedPlant); 
    }

    [HttpPost("{id}/add-production-data")]
    public async Task<ActionResult<IEnumerable<ProductionData>>> AddProductionData(int id, [FromBody] IEnumerable<ProductionData> newProductionData)
    {
        var plant = await _context.SolarPlants.FindAsync(id);
        if (plant == null)
        {
            return NotFound();
        }

        foreach (var data in newProductionData)
        {
            data.SolarPowerPlantId = id;
            _context.ProductionData.Add(data);
        }

        await _context.SaveChangesAsync();
        return Ok(newProductionData);
    }


    [HttpPut("{id}/soft-delete")]
    public async Task<ActionResult> SoftDeleteSolarPlant(int id)
    {
        var plant = await _context.SolarPlants.FindAsync(id);
        if (plant == null)
        {
            return NotFound();
        }

        plant.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent(); 
    }
}
