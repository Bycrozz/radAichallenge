using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class MobileFoodTrucksController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MobileFoodTrucksController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("searchByName")]
    public IActionResult SearchByName(string name, string? status = null)
    {
        var query = _context.MobileFoodTrucks.AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(f => f.Status == status);
        }

        var results = query.ToList().Where(f => f.Applicant != null && f.Applicant.Contains(name, StringComparison.OrdinalIgnoreCase));

        return Ok(results);
    }

    [HttpGet("searchByStreet")]
    public IActionResult SearchByStreet(string street)
    {
        var results = _context.MobileFoodTrucks
            .ToList()
            .Where(f => f.Address != null && f.Address.Contains(street, StringComparison.OrdinalIgnoreCase));

        return Ok(results);
    }

    [HttpGet("nearestFoodTrucks")]
    public IActionResult NearestFoodTrucks(double latitude, double longitude, string status = "APPROVED")
    {
        var query = _context.MobileFoodTrucks.AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(f => f.Status == status);
        }

        var results = query
            .OrderBy(f => (f.Latitude - latitude) * (f.Latitude - latitude) + (f.Longitude - longitude) * (f.Longitude - longitude))
            .Take(5)
            .ToList();

        return Ok(results);
    }
}
