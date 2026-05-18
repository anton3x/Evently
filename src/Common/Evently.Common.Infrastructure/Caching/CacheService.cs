using System.Buffers;
using System.Text.Json;
using Evently.Common.Application.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace Evently.Common.Infrastructure.Caching;

internal sealed class CacheService(IDistributedCache cache) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken token = default)
    {
        byte[]? bytes = await cache.GetAsync(key, token);
        return bytes is null ? default : Deserialize<T>(bytes);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken token = default)
    {
        byte[] bytes = Serialize(value);

        return cache.SetAsync(key, bytes, CacheOptions.Create(expiration), token);
    }

    public Task RemoveAsync(string key, CancellationToken token = default)
    {
        return cache.RemoveAsync(key, token);
    }

    private static T Deserialize<T>(byte[] bytes)
    {
        return JsonSerializer.Deserialize<T>(bytes);
    }

    private static byte[] Serialize<T>(T value)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using var writter = new Utf8JsonWriter(buffer);
        JsonSerializer.Serialize(writter, value);
        return buffer.WrittenSpan.ToArray();
    }
}
