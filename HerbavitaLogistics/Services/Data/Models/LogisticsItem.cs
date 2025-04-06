public class LogisticsItem
{
    public string Id { get; set; }
    public string Barcode { get; set; }
    public string Name { get; set; }
    public bool IsDiscontinued { get; set; }
    public DateTime LastScanned { get; set; }
}