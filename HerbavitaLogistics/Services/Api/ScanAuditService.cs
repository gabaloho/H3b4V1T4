// Services/ScanAuditService.cs
public class ScanAuditService
{
    private readonly LogisticsDbContext _db; // Or API client

    public async Task LogScanAsync(string barcode, string employeeId, DateTime scanTime)
    {
        var auditRecord = new ScanAudit
        {
            Id = Guid.NewGuid(),
            Barcode = barcode,
            EmployeeId = employeeId,
            ScanTime = scanTime,
            Location = await GetDeviceLocationAsync()
        };

        await _db.ScanAudits.AddAsync(auditRecord);
        await _db.SaveChangesAsync();
    }

    private async Task<string> GetDeviceLocationAsync()
    {
        // Implement using MAUI Essentials or Zebra location APIs
        return "Warehouse-A";
    }
}