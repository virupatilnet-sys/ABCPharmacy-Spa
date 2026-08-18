using System.Text.Json;
using PharmacySpa.Api.Models;

namespace PharmacySpa.Api.Services;

public sealed class MedicineRepository(IWebHostEnvironment environment)
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly string _path = Path.Combine(environment.ContentRootPath, "Data", "pharmacy-data.json");
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true, PropertyNameCaseInsensitive = true };

    public async Task<T> MutateAsync<T>(Func<PharmacyData, T> action, CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct);
        try
        {
            var data = await ReadUnsafeAsync(ct);
            var result = action(data);
            await WriteUnsafeAsync(data, ct);
            return result;
        }
        finally { _lock.Release(); }
    }

    public async Task<PharmacyData> ReadAsync(CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct);
        try { return await ReadUnsafeAsync(ct); }
        finally { _lock.Release(); }
    }

    private async Task<PharmacyData> ReadUnsafeAsync(CancellationToken ct)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        if (!File.Exists(_path)) return new PharmacyData();
        await using var stream = File.OpenRead(_path);
        return await JsonSerializer.DeserializeAsync<PharmacyData>(stream, JsonOptions, ct) ?? new PharmacyData();
    }

    private async Task WriteUnsafeAsync(PharmacyData data, CancellationToken ct)
    {
        var temporaryPath = _path + ".tmp";
        await using (var stream = File.Create(temporaryPath))
            await JsonSerializer.SerializeAsync(stream, data, JsonOptions, ct);
        File.Move(temporaryPath, _path, true); // atomic replacement on the same volume
    }
}
