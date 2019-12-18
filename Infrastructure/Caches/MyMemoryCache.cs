using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using SaasKit.Multitenancy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Infrastructure.Caches
{
    public class MyMemoryCache: IMyCache
    {
        private readonly IMemoryCache _cache;
        private readonly ConcurrentDictionary<object, ICacheEntry> _cacheEntries = new ConcurrentDictionary<object, ICacheEntry>();

        public MyMemoryCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Dispose()
        {
            this._cache.Dispose();
        }

        private void PostEvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            if (reason != EvictionReason.Replaced)
                this._cacheEntries.TryRemove(key, out var _);
        }

        /// <inheritdoc cref="IMemoryCache.TryGetValue"/>
        public bool TryGetValue(object key, out object value)
        {
            return this._cache.TryGetValue(key, out value);
        }

        public bool TryGetValue<T>(object key, out T value)
        {
            return this._cache.TryGetValue<T>(key, out value);
        }

        /// <summary>
        /// Create or overwrite an entry in the cache and add key to Dictionary.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        /// <returns>The newly created <see cref="T:Microsoft.Extensions.Caching.Memory.ICacheEntry" /> instance.</returns>
        public ICacheEntry CreateEntry(object key)
        {
            var entry = this._cache.CreateEntry(key);
            entry.RegisterPostEvictionCallback(this.PostEvictionCallback);
            this._cacheEntries.AddOrUpdate(key, entry, (o, cacheEntry) =>
            {
                cacheEntry.Value = entry;
                return cacheEntry;
            });
            return entry;
        }

        /// <inheritdoc cref="IMemoryCache.Remove"/>
        public void Remove(object key)
        {
            this._cache.Remove(key);
        }

        /// <summary>
        /// Removes items by pattern
        /// </summary>
        /// <param name="pattern">pattern</param>
        public virtual void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = new List<string>();

            foreach (var item in _cacheEntries)
            {
                if (regex.IsMatch((string)item.Key))
                    keysToRemove.Add((string)item.Key);
            }

            foreach (string key in keysToRemove)
            {
                Remove(key);
            }
        }

        public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
        {
            return this._cacheEntries.Select(pair => new KeyValuePair<object, object>(pair.Key, pair.Value.Value)).GetEnumerator();
        }

        /// <summary>
        /// Gets keys of all items in MemoryCache.
        /// </summary>
        public IEnumerator<object> Keys => this._cacheEntries.Keys.GetEnumerator();
    }
}
