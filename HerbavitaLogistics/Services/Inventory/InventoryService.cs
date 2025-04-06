// Services/InventoryService.cs
public class InventoryService
{
    private readonly HttpClient _httpClient;
    private readonly List<string> _processedScans = new();
    private readonly TimeSpan _duplicateWindow = TimeSpan.FromMinutes(5);

    public InventoryService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<ItemValidationResult> VerifyProductAsync(string barcode)
    {
        // Check for recent duplicate scans
        if (_processedScans.Contains(barcode))
            return new ItemValidationResult(barcode, "Duplicate scan", false);

        // API call to logistics database
        var response = await _httpClient.GetAsync($"/api/items/validate?barcode={barcode}");

        if (!response.IsSuccessStatusCode)
            return new ItemValidationResult(barcode, "Network error", false);

        var item = await response.Content.ReadFromJsonAsync<LogisticsItem>();

        if (item == null)
            return new ItemValidationResult(barcode, "Not in database", false);

        // Add to processed scans (with auto-cleanup)
        _processedScans.Add(barcode);
        CleanOldScans();

        return new ItemValidationResult(item.Id, item.Name, true);
    }

    private void CleanOldScans()
    {
        // Implement if using persistent storage
    }
}