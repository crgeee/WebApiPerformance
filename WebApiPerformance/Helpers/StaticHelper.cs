using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Caching;
using WebApiPerformance.Models;

namespace WebApiPerformance.Helpers
{
    public static class StaticHelper
    {
        /// <summary>
        /// Create test data
        /// </summary>
        /// <param name="numberOfRecords"></param>
        /// <returns></returns>
        public static List<TestViewModel> CreateTestData(int numberOfRecords)
        {
            var list = new List<TestViewModel>();
            for (int i = 0; i < numberOfRecords; i++)
            {
                list.Add(new TestViewModel(i + " test value 1234"));
            }
            return list;
        }


        /// <summary>
        /// Get Data with memory caching
        /// </summary>
        /// <param name="numOfRecords"></param>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public static List<TestViewModel> GetDataWithMemCaching(int numOfRecords, string cacheKey)
        {
            Debug.WriteLine("GetDataWithMemCaching() hit");
            var memCacher = new MemoryCacher<List<TestViewModel>>(new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(20) });
            List<TestViewModel> results;
            memCacher.TryGetAndSet(cacheKey, () => StaticHelper.CreateTestData(numOfRecords), out results);
            return results;
        }
    }
}