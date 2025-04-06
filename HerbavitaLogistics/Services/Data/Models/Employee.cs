public class Employee
{
    public string Id { get; set; }

    [Required]
    public string BadgeId { get; set; }

    [Required]
    public string Name { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation property
    public ICollection<ScanAudit> Scans { get; set; }
}