using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UpriseMidTask.Data;
using UpriseMidTask.DTOs;
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
        _logger.LogInformation($"Creating a new solar plant {plant.Id}");
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
        _logger.LogInformation($"Get solar plant {plant.Id}");
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

        _logger.LogInformation($"Getting all solar plants.");
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
        _logger.LogInformation($"Updating solar plant {plant.Id}");
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
        _logger.LogInformation($"Adding production data for solar plant {plant.Id}");
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
        _logger.LogInformation($"Deleting solar plant {plant.Id}");
        return NoContent(); 
    }

    [HttpGet("{id}/get-production-data")]
    public async Task<ActionResult<IEnumerable<ProductionData>>> GetProductionData(
    int id,
    [FromQuery] DateTime startDate,
    [FromQuery] DateTime endDate,
    [FromQuery] int granularity = 15,
    [FromQuery] bool isForecast = false)
    {
        var validGranularities = new[] { 15, 60 };
        if (!validGranularities.Contains(granularity))
        {
            return BadRequest("Allowed granularity values: 15 (15 minutes) and 60 (1 hour)");
        }

        var plant = await _context.SolarPlants
            .Include(p => p.ProductionData)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (plant == null)
        {
            return NotFound("Solar plant not found.");
        }

        if (plant.ProductionData == null)
        {
            return NotFound("No production data for given plant.");
        }

        var filteredData = plant.ProductionData
            .Where(d => d.Timestamp >= startDate && d.Timestamp <= endDate)
            .Where(d => d.IsForecast == isForecast) // Ensure actual/forecasted filtering
            .OrderBy(d => d.Timestamp)
            .ToList();

        var aggregatedData = filteredData
            .GroupBy(d => new
            {
                Interval = new TimeSpan(
                    ((long)(d.Timestamp - startDate).TotalMinutes / granularity) * granularity * 600000000L
                )
            })
            .Select(g => new AggregateProductionData
            {
                Timestamp = startDate.AddMinutes(g.Key.Interval.TotalMinutes),
                TotalProduction = g.Sum(d => d.Production), 
                AverageProduction = g.Average(d => d.Production)
            })
            .ToList();

        _logger.LogInformation($"Fetching timeseries data for solar plant {plant.Id}");
        return Ok(aggregatedData);
    }
}
