public class ScannerStateService : IDisposable
{
    private readonly IApiService _api;
    private readonly AuthState _auth;
    private readonly ConcurrentDictionary<string, DateTime> _scanCache = new();

    public ScannerStateService(IApiService api, AuthState auth)
    {
        _api = api;
        _auth = auth;
        MessagingCenter.Subscribe<object, string>(this, "ScanReceived", HandleScan);
    }

    private async void HandleScan(object sender, string scanData)
    {
        if (_scanCache.TryGetValue(scanData, out var lastScan) &&
            (DateTime.UtcNow - lastScan) < TimeSpan.FromMinutes(5)) return;

        _scanCache[scanData] = DateTime.UtcNow;

        if (_auth.CurrentEmployee == null)
            await _api.ValidateEmployeeAsync(scanData);
        else
            await _api.ValidateItemAsync(scanData);
    }

    public void Dispose() => MessagingCenter.Unsubscribe<object>(this, "ScanReceived");
}