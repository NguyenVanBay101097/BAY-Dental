using ApplicationCore.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Caches
{
    public static class MyMemoryCacheExtensions
    {
        public static T Set<T>(this IMyCache cache, object key, T value)
        {
            var entry = cache.CreateEntry(key);
            entry.Value = value;
            entry.Dispose();

            return value;
        }

        public static T Set<T>(this IMyCache cache, object key, T value, CacheItemPriority priority)
        {
            var entry = cache.CreateEntry(key);
            entry.Priority = priority;
            entry.Value = value;
            entry.Dispose();

            return value;
        }

        public static T Set<T>(this IMyCache cache, object key, T value, DateTimeOffset absoluteExpiration)
        {
            var entry = cache.CreateEntry(key);
            entry.AbsoluteExpiration = absoluteExpiration;
            entry.Value = value;
            entry.Dispose();

            return value;
        }

        public static T Set<T>(this IMyCache cache, object key, T value, TimeSpan absoluteExpirationRelativeToNow)
        {
            var entry = cache.CreateEntry(key);
            entry.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
            entry.Value = value;
            entry.Dispose();

            return value;
        }

        public static T Set<T>(this IMyCache cache, object key, T value, MemoryCacheEntryOptions options)
        {
            using (var entry = cache.CreateEntry(key))
            {
                if (options != null)
                    entry.SetOptions(options);

                entry.Value = value;
            }

            return value;
        }

        public static TItem GetOrCreate<TItem>(this IMyCache cache, object key, Func<ICacheEntry, TItem> factory)
        {
            if (!cache.TryGetValue(key, out var result))
            {
                var entry = cache.CreateEntry(key);
                result = factory(entry);
                entry.SetValue(result);
                entry.Dispose();
            }

            return (TItem)result;
        }

        public static async Task<TItem> GetOrCreateAsync<TItem>(this IMyCache cache, object key, Func<ICacheEntry, Task<TItem>> factory)
        {
            if (!cache.TryGetValue(key, out object result))
            {
                var entry = cache.CreateEntry(key);
                result = await factory(entry);
                entry.SetValue(result);
                entry.Dispose();
            }

            return (TItem)result;
        }
    }
}
