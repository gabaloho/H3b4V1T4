// Controllers/ScansController.cs
[ApiController]
[Route("api/[controller]")]
public class ScansController : ControllerBase
{
    private readonly LogisticsDbContext _db;
    private readonly ILogger<ScansController> _logger;

    public ScansController(LogisticsDbContext db, ILogger<ScansController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpPost("log")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LogScan([FromBody] ScanLogRequest request)
    {
        try
        {
            // Validation
            if (!await _db.Employees.AnyAsync(e => e.Id == request.EmployeeId))
                return BadRequest(new { Message = "Invalid employee ID" });

            var auditRecord = new ScanAudit
            {
                Id = Guid.NewGuid().ToString(),
                Barcode = request.Barcode,
                EmployeeId = request.EmployeeId,
                ScanTime = request.ScanTime ?? DateTime.UtcNow,
                Location = request.Location ?? "Unknown"
            };

            await _db.ScanAudits.AddAsync(auditRecord);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Scan logged: {Barcode} by {EmployeeId}",
                request.Barcode, request.EmployeeId);

            return Ok(new { AuditId = auditRecord.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging scan for {EmployeeId}", request.EmployeeId);
            return StatusCode(500, new { Message = "Failed to log scan" });
        }
    }

    // Batch processing
    [HttpPost("log/batch")]
    public async Task<IActionResult> LogScans([FromBody] List<ScanLogRequest> requests)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var audits = requests.Select(r => new ScanAudit
            {
                Id = Guid.NewGuid().ToString(),
                Barcode = r.Barcode,
                EmployeeId = r.EmployeeId,
                ScanTime = r.ScanTime ?? DateTime.UtcNow,
                Location = r.Location ?? "BatchImport"
            });

            await _db.ScanAudits.AddRangeAsync(audits);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { Count = requests.Count });
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}