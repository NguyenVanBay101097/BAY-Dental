using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ApplicationCore.Utilities
{
    public static class LockUtils
    {
        private static ConcurrentDictionary<object, SemaphoreSlim> _locks = new ConcurrentDictionary<object, SemaphoreSlim>();

        public static ConcurrentDictionary<object, SemaphoreSlim> Locks
        {
            get
            {
                return _locks;
            }
        }

        public static SemaphoreSlim Get(string key)
        {
            return Locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
        }

        public static SemaphoreSlim Get(object key)
        {
            return Locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
        }
    }
}
