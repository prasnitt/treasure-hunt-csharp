using System;
using Microsoft.Extensions.Caching.Memory;
using TreasurehuntApi.Model;

namespace TreasurehuntApi.Service
{
    public class InMemoryDataService
    {
        private readonly IMemoryCache cache;

        public static string GameDataKey = "GameDataKey";
        public static string GameStateKey = "GameStateKey";
        public static string TeamMemberNamesKey = "TeamMemberNamesKey";

        public InMemoryDataService(IMemoryCache memoryCache)
        {
            cache = memoryCache;
        }

        public void AddItemToCache(string key, object item)
        {
            // Add item to cache with a specific key
            cache.Set(key, item);

            // Optionally, you can specify cache options such as expiration time
            // cache.Set(key, item, TimeSpan.FromMinutes(30));
        }

        public object GetItemFromCache(string key)
        {
            // Retrieve item from cache based on the specified key
            return cache.Get(key);
        }

        public void RemoveItemFromCache(string key)
        {
            // Remove item from cache based on the specified key
            cache.Remove(key);
        }
    }
}
