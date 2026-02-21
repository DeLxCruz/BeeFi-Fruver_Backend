using Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

// TODO: Reemplazar con Redis para producción distribuida
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<MemoryCacheService> _logger;

    // SemaphoreSlim por key para evitar cache stampede
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, SemaphoreSlim>
        _locks = new();

    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

    public MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        where T : class
    {
        _cache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default)
        where T : class
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration
        };
        _cache.Set(key, value, options);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        _cache.Remove(key);
        _logger.LogDebug("Cache invalidated for key: {Key}", key);
        return Task.CompletedTask;
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken ct = default)
        where T : class
    {
        // Fast path: hit
        if (_cache.TryGetValue(key, out T? cached) && cached is not null)
            return cached;

        // Slow path: acquire per-key lock to avoid stampede
        var semaphore = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(ct);
        try
        {
            // Double-check after acquiring lock
            if (_cache.TryGetValue(key, out cached) && cached is not null)
                return cached;

            _logger.LogDebug("Cache miss for key: {Key}, invoking factory", key);
            var value = await factory();

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration
            };
            _cache.Set(key, value, options);
            return value;
        }
        finally
        {
            semaphore.Release();
        }
    }
}
