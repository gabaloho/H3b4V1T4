// Services/NetworkMonitor.cs
public class NetworkMonitor : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly IApiService _apiService;

    public NetworkMonitor(IApiService apiService)
    {
        _apiService = apiService;
        Connectivity.ConnectivityChanged += OnConnectivityChanged;
    }

    private async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        if (e.NetworkAccess == NetworkAccess.Internet)
        {
            await LocalDataStore.SyncPendingScansAsync(_apiService);
        }
    }

    public static bool IsConnected =>
        Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
}