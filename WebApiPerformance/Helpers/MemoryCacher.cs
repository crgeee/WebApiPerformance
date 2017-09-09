using System;
using System.Runtime.Caching;

namespace WebApiPerformance.Helpers
{
    public class MemoryCacher<T>
    {
        private readonly MemoryCache _memoryCache;
        private readonly CacheItemPolicy _defaultPolicy;

        /// <summary>
        /// Instantiate helper class with default policy and MemoryCache class.
        /// </summary>
        /// <param name="policy"></param>
        public MemoryCacher(CacheItemPolicy policy = null)
        {
            this._memoryCache = MemoryCache.Default;
            this._defaultPolicy = policy ?? new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(30)
            };
        }

        /// <summary>
        /// Add a new CacheItem into MemoryCache by key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Add(string key, object value)
        {
            return _memoryCache.Add(key, value, _defaultPolicy);
        }

        /// <summary>
        /// Remove an existing CacheItem from MemoryCache by key.
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            if (_memoryCache.Contains(key))
            {
                _memoryCache.Remove(key);
            }
        }

        /// <summary>
        /// Get an existing Cache Item from MemoryCache.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key)
        {
            return _memoryCache.Get(key);
        }

        /// <summary>
        /// Set (Remove & Add) a Cache Item into MemoryCache.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cacheItem"></param>
        public void Set(string key, T cacheItem)
        {
            _memoryCache.Set(key, cacheItem, _defaultPolicy);
        }

        /// <summary>
        /// Try to get by key with type and return null if not.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="returnItem"></param>
        /// <returns></returns>
        public bool TryGet(string key, out T returnItem)
        {
            returnItem = (T)this.Get(key);
            return returnItem != null;
        }

        /// <summary>
        /// Try to get by key with type. If null, use provided function to get and 
        /// then set in MemoryCache.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="getData"></param>
        /// <param name="returnData"></param>
        /// <returns></returns>
        public bool TryGetAndSet(string key, Func<T> getData, out T returnData)
        {
            if (TryGet(key, out returnData))
            {
                return true;
            }
            returnData = getData();
            this.Set(key, returnData);
            return returnData != null;
        }        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="getData"></param>
        /// <returns></returns>
        public bool TrySet(string key, Func<T> getData)
        {
            var returnData = getData();
            this.Set(key, returnData);
            return returnData != null;
        }
    }
}