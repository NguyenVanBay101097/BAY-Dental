using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Interfaces
{
    public interface IMyCache : IMemoryCache
    {
        /// <summary>
        /// Clears all cache entries.
        /// </summary>
        void Clear();
    }
}
