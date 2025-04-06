public class ScanLogRequest
{
    [Required]
    public string Barcode { get; set; }

    [Required]
    public string EmployeeId { get; set; }

    public DateTime? ScanTime { get; set; }

    [StringLength(100)]
    public string Location { get; set; }
}