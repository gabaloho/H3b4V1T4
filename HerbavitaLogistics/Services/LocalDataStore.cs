// Services/LocalDataStore.cs
public static class LocalDataStore
{
    private static SQLiteAsyncConnection _database;

    public static async Task Init()
    {
        if (_database != null) return;

        _database = new SQLiteAsyncConnection(
            Path.Combine(FileSystem.AppDataDirectory, "scans.db"),
            SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache);

        await _database.CreateTableAsync<PendingScan>();
    }

    public static async Task QueueScanAsync(string scanData)
    {
        await Init();
        await _database.InsertAsync(new PendingScan
        {
            ScanData = scanData,
            CreatedAt = DateTime.UtcNow,
            IsProcessed = false
        });
    }

    public static async Task SyncPendingScansAsync(IApiService apiService)
    {
        await Init();
        var pending = await _database.Table<PendingScan>()
            .Where(s => !s.IsProcessed)
            .ToListAsync();

        foreach (var scan in pending)
        {
            try
            {
                if (scan.ScanData.StartsWith("BADGE_"))
                {
                    await apiService.AuthenticateAsync(scan.ScanData);
                }
                else
                {
                    await apiService.ValidateItemAsync(scan.ScanData);
                }

                scan.IsProcessed = true;
                await _database.UpdateAsync(scan);
            }
            catch { /* Retry next sync */ }
        }
    }

    [Table("PendingScans")]
    public class PendingScan
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string ScanData { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsProcessed { get; set; }
    }
}