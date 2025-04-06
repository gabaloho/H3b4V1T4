// Services/ScannerStateService.cs
public class ScannerStateService
{
    private readonly EmployeeService _employeeService;
    private readonly ScanAuditService _auditService;
    private readonly AuthState _authState;

    public async Task HandleBadgeScan(string badgeId)
    {
        var result = await _employeeService.VerifyEmployeeAsync(badgeId);

        if (result.Employee == null)
        {
            // Show error
            return;
        }

        _authState.Login(result.Employee);
    }

    public async Task HandleItemScan(string barcode)
    {
        if (_authState.CurrentEmployee == null) return;

        await _auditService.LogScanAsync(
            barcode,
            _authState.CurrentEmployee.Id,
            DateTime.UtcNow
        );
    }
}


public class ScannerStateService
{
    private readonly InventoryService _inventory;
    private readonly IScanContext _scanContext;
    private readonly Dictionary<string, DateTime> _scanHistory = new();

    public ScannerStateService(InventoryService inventory, IScanContext scanContext)
    {
        _inventory = inventory;
        _scanContext = scanContext;
        _scanContext.ScanReceived += HandleScan;
    }

    private async void HandleScan(string barcode)
    {
        // Skip duplicates within 5 minutes
        if (_scanHistory.TryGetValue(barcode, out var lastScan) &&
            DateTime.UtcNow - lastScan < TimeSpan.FromMinutes(5))
        {
            WeakReferenceMessenger.Default.Send(new ScanRejectedMessage(barcode, "Duplicate scan"));
            return;
        }

        _scanHistory[barcode] = DateTime.UtcNow;
        var result = await _inventory.VerifyProductAsync(barcode);

        WeakReferenceMessenger.Default.Send(new ItemValidatedMessage(result));
    }
}



// Services/ScannerStateService.cs
public class ScannerStateService : IDisposable
{
    private readonly IApiService _apiService;
    private readonly IScanContext _scanContext;
    private bool _isProcessing;

    public ScannerStateService(IApiService apiService, IScanContext scanContext)
    {
        _apiService = apiService;
        _scanContext = scanContext;
        _scanContext.ScanReceived += HandleScan;
    }

    private async void HandleScan(string scanData)
    {
        if (_isProcessing) return;
        _isProcessing = true;

        try
        {
            // Determine scan type (badge vs item) via prefix or length
            if (scanData.StartsWith("BADGE_"))
            {
                var authResult = await _apiService.AuthenticateAsync(scanData);
                if (!authResult.IsSuccess)
                {
                    // Show error in UI
                    WeakReferenceMessenger.Default.Send(new ScanFailedMessage("Invalid badge"));
                }
            }
            else
            {
                var itemResult = await _apiService.ValidateItemAsync(scanData);
                WeakReferenceMessenger.Default.Send(new ItemScannedMessage(itemResult));
            }
        }
        catch (Exception ex)
        {
            WeakReferenceMessenger.Default.Send(new ScanFailedMessage("Network error"));
            // Optionally store scan for later sync
            await LocalDataStore.QueueScanAsync(scanData);
        }
        finally
        {
            _isProcessing = false;
        }
    }

    public void Dispose() => _scanContext.ScanReceived -= HandleScan;
}

ScannerStateService.cs
// Shared/Services/ScannerStateService.cs
public class ScannerStateService : IDisposable
{
    private readonly IScanContext _scanContext;
    private bool _isScannerSuspended;

    public ScannerStateService(IScanContext scanContext)
    {
        _scanContext = scanContext;
        _scanContext.ScanReceived += HandleScan;
    }

    public void SuspendScanner()
    {
        if (_isScannerSuspended) return;

        var extras = new Bundle();
        extras.PutString("PROFILE_NAME", "WarehouseApp");
        extras.PutString("PLUGIN_NAME", "BARCODE");
        extras.PutString("SUSPEND_PLUGIN", "true");

        SendDataWedgeIntent("com.symbol.datawedge.api.ACTION", extras);
        _isScannerSuspended = true;
    }

    public void ResumeScanner()
    {
        if (!_isScannerSuspended) return;

        var extras = new Bundle();
        extras.PutString("PROFILE_NAME", "WarehouseApp");
        extras.PutString("PLUGIN_NAME", "BARCODE");
        extras.PutString("RESUME_PLUGIN", "true");

        SendDataWedgeIntent("com.symbol.datawedge.api.ACTION", extras);
        _isScannerSuspended = false;
    }

    private void HandleScan(string scanData)
    {
        // Process scan data
    }

    public void Dispose()
    {
        _scanContext.ScanReceived -= HandleScan;
        ResumeScanner(); // Ensure scanner is active when service stops
    }
}