using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace SmartCache
{
    public static class LocalCacheHelper
    {
        /// <summary>
        /// 获取数据缓存
        /// </summary>
        /// <param name="CacheKey">键</param>
        public static object GetCache(string key)
        {
            Cache objCache = HttpRuntime.Cache;
            return objCache[key];
        }
        /// <summary>
        /// 设置数据缓存（如果存在则更新）
        /// </summary>
        public static void SetCache(string key, object objObject)
        {
            Cache objCache = HttpRuntime.Cache;
            objCache.Insert(key, objObject);
        }
        /// <summary>
        /// 设置数据缓存（如果存在则更新）
        /// </summary>
        public static void SetCache(string key, object objObject, TimeSpan timeout)
        {
            Cache objCache = HttpRuntime.Cache;
            objCache.Insert(key, objObject, null, DateTime.MaxValue, timeout, System.Web.Caching.CacheItemPriority.NotRemovable, null);
        }
        /// <summary>
        /// 设置数据缓存（如果存在则更新）
        /// </summary>
        public static void SetCache(string key, object objObject, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            Cache objCache = HttpRuntime.Cache;
            objCache.Insert(key, objObject, null, absoluteExpiration, slidingExpiration);
        }
        /// <summary>
        /// 移除指定数据缓存
        /// </summary>
        public static void RemoveCache(string key)
        {
            Cache _cache = HttpRuntime.Cache;
            _cache.Remove(key);
        }

        /// <summary>
        /// 移除全部缓存
        /// </summary>
        public static void RemoveAllCache()
        {
            Cache _cache = HttpRuntime.Cache;
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            while (CacheEnum.MoveNext())
            {
                _cache.Remove(CacheEnum.Key.ToString());
            }
        }
    }
}
