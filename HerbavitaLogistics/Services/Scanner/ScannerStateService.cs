// Services/Scanner/ScannerStateService.cs
public class ScannerStateService : IDisposable
{
    private readonly IApiService _api;
    private readonly AuthState _auth;
    private readonly ScanAuditService _audit;
    private readonly ConcurrentDictionary<string, DateTime> _scanHistory = new();
    private bool _isProcessing;

    public ScannerStateService(IApiService api, AuthState auth, ScanAuditService audit)
    {
        _api = api;
        _auth = auth;
        _audit = audit;
        MessagingCenter.Subscribe<object, string>(this, "ScanReceived", HandleScan);
    }

    private async void HandleScan(object sender, string scanData)
    {
        // 1. Prevent duplicate processing
        if (_isProcessing) return;
        _isProcessing = true;

        try
        {
            // 2. Check for duplicate scans (5-minute window)
            if (_scanHistory.TryGetValue(scanData, out var lastScan) &&
                (DateTime.UtcNow - lastScan) < TimeSpan.FromMinutes(5))
            {
                MessagingCenter.Send(this, "ScanRejected", "Duplicate scan");
                return;
            }

            // 3. Process scan based on type
            if (scanData.StartsWith("BADGE_"))
            {
                await ProcessBadgeScan(scanData);
            }
            else
            {
                await ProcessItemScan(scanData);
            }
        }
        catch (Exception ex)
        {
            MessagingCenter.Send(this, "ScanFailed", $"Error: {ex.Message}");
        }
        finally
        {
            _isProcessing = false;
        }
    }

    private async Task ProcessBadgeScan(string badgeId)
    {
        var result = await _api.ValidateEmployeeAsync(badgeId);
        if (result.IsSuccess)
        {
            _auth.Login(result.Employee);
            MessagingCenter.Send(this, "AuthSuccess", result.Employee.Name);
        }
        else
        {
            MessagingCenter.Send(this, "AuthFailed", result.ErrorMessage);
        }
    }

    private async Task ProcessItemScan(string barcode)
    {
        if (_auth.CurrentEmployee == null) return;

        var result = await _api.ValidateItemAsync(barcode);
        await _audit.LogScanAsync(barcode, _auth.CurrentEmployee.Id);

        MessagingCenter.Send(this, "ItemScanned", result);
    }

    public void Dispose()
    {
        MessagingCenter.Unsubscribe<object>(this, "ScanReceived");
    }
}