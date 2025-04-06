// Services/Data/LocalDataStore.cs
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

public class LocalDataStore
{
    private readonly SQLiteAsyncConnection _database;

    public LocalDataStore()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "scans.db");
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<PendingScan>().Wait();
    }

    public async Task QueueScanAsync(string scanData)
    {
        var scan = new PendingScan
        {
            ScanData = scanData,
            CreatedAt = DateTime.UtcNow,
            IsProcessed = false
        };
        await _database.InsertAsync(scan);
    }

    public async Task<List<PendingScan>> GetPendingScansAsync()
    {
        return await _database.Table<PendingScan>()
            .Where(s => !s.IsProcessed)
            .ToListAsync();
    }

    public async Task MarkAsProcessedAsync(int scanId)
    {
        var scan = await _database.Table<PendingScan>()
            .FirstOrDefaultAsync(s => s.Id == scanId);

        if (scan != null)
        {
            scan.IsProcessed = true;
            await _database.UpdateAsync(scan);
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