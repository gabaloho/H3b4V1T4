// Controllers/EmployeesController.cs
[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly LogisticsDbContext _db;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(LogisticsDbContext db, ILogger<EmployeesController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet("validate")]
    [ProducesResponseType(typeof(Employee), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ValidateEmployee(
        [FromQuery] string badgeId,
        [FromQuery] bool includeRecentScans = false)
    {
        try
        {
            var query = _db.Employees
                .Where(e => e.BadgeId == badgeId && e.IsActive);

            if (includeRecentScans)
            {
                query = query.Include(e => e.Scans
                    .OrderByDescending(s => s.ScanTime)
                    .Take(5));
            }

            var employee = await query.FirstOrDefaultAsync();

            if (employee == null)
            {
                _logger.LogWarning("Invalid badge attempt: {BadgeId}", badgeId);
                return NotFound(new { Message = "Employee not found or inactive" });
            }

            return Ok(employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating employee {BadgeId}", badgeId);
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }

    // Additional endpoints...
    [HttpGet("{id}/scans")]
    public async Task<IActionResult> GetEmployeeScans(string id, [FromQuery] DateTime? fromDate)
    {
        var query = _db.ScanAudits
            .Where(s => s.EmployeeId == id);

        if (fromDate.HasValue)
            query = query.Where(s => s.ScanTime >= fromDate);

        var scans = await query.ToListAsync();
        return Ok(scans);
    }
}