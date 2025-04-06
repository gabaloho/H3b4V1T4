// EmployeesController.cs
[HttpGet("validate")]
public async Task<IActionResult> ValidateEmployee([FromQuery] string badgeId)
{
    var employee = await _db.Employees
        .FirstOrDefaultAsync(e => e.BadgeId == badgeId && e.IsActive);

    return employee == null
        ? NotFound()
        : Ok(employee);
}

// ScansController.cs
[HttpPost("log")]
public async Task<IActionResult> LogScan([FromBody] ScanLogRequest request)
{
    var auditRecord = new ScanAudit
    {
        Barcode = request.Barcode,
        EmployeeId = request.EmployeeId,
        ScanTime = request.ScanTime,
        Location = request.Location
    };

    _db.ScanAudits.Add(auditRecord);
    await _db.SaveChangesAsync();

    return Ok();
}