namespace Application.Common.Interfaces;

/// <summary>
/// Servicio de caché genérico. Implementación actual: IMemoryCache (in-process).
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        where T : class;

    Task SetAsync<T>(string key, T value,
        TimeSpan? expiration = null,
        CancellationToken ct = default)
        where T : class;

    Task RemoveAsync(string key, CancellationToken ct = default);

    /// <summary>
    /// Cache-aside: devuelve el valor cacheado o lo crea con <paramref name="factory"/>.
    /// Usa SemaphoreSlim internamente para evitar cache stampede.
    /// </summary>
    Task<T> GetOrCreateAsync<T>(string key,
        Func<Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken ct = default)
        where T : class;
}
