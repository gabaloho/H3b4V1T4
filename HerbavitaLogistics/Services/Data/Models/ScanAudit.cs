public class ScanAudit
{
    public Guid Id { get; set; }
    public string Barcode { get; set; }
    public string EmployeeId { get; set; }
    public DateTime ScanTime { get; set; }
    public string Location { get; set; }
    
    [ForeignKey("EmployeeId")]
    public Employee Employee { get; set; }
}