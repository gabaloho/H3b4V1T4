// Platforms/Android/DataWedgeIntentService.cs
[Service]
public class DataWedgeIntentService : Service
{
    private static readonly object _lock = new object();
    private static Queue<Intent> _intentQueue = new Queue<Intent>();

    public override IBinder OnBind(Intent intent) => null;

    public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
    {
        lock (_lock)
        {
            _intentQueue.Enqueue(intent);
            if (_intentQueue.Count == 1) ProcessNextIntent();
        }
        return StartCommandResult.Sticky;
    }

    private void ProcessNextIntent()
    {
        if (_intentQueue.Count == 0) return;

        var currentIntent = _intentQueue.Peek();
        // Process intent here

        // When done:
        lock (_lock)
        {
            _intentQueue.Dequeue();
            ProcessNextIntent();
        }
    }
}