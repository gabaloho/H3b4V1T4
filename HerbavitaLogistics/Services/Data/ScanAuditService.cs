// Services/Data/ScanAuditService.cs
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

public class ScanAuditService
{
    private readonly LogisticsDbContext _context;

    public ScanAuditService(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task LogScanAsync(string barcode, string employeeId, string location = null)
    {
        var audit = new ScanAudit
        {
            Id = Guid.NewGuid().ToString(),
            Barcode = barcode,
            EmployeeId = employeeId,
            ScanTime = DateTime.UtcNow,
            Location = location ?? await GetDeviceLocationAsync()
        };

        await _context.ScanAudits.AddAsync(audit);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ScanAudit>> GetScansByEmployeeAsync(string employeeId, DateTime? fromDate = null)
    {
        var query = _context.ScanAudits
            .Where(s => s.EmployeeId == employeeId);

        if (fromDate.HasValue)
            query = query.Where(s => s.ScanTime >= fromDate);

        return await query.OrderByDescending(s => s.ScanTime).ToListAsync();
    }

    private async Task<string> GetDeviceLocationAsync()
    {
        // Implement using MAUI Essentials or Zebra-specific APIs
        try
        {
            var location = await Geolocation.GetLastKnownLocationAsync();
            return location != null ?
                $"{location.Latitude},{location.Longitude}" :
                "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }
}

// Models/ScanAudit.cs
public class ScanAudit
{
    public string Id { get; set; }
    public string Barcode { get; set; }
    public string EmployeeId { get; set; }
    public DateTime ScanTime { get; set; }
    public string Location { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual Employee Employee { get; set; }
}