using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using WebApiPerformance.DAL;
using WebApiPerformance.Filters;
using WebApiPerformance.Helpers;
using WebApiPerformance.Models;

namespace WebApiPerformance.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly TestContext _dbContext = new TestContext();

        // GET api/values
        [HttpGet]
        public List<TestViewModel> Get()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Debug.WriteLine("Begin: Get()");

            var data = StaticHelper.CreateTestData(60000);

            Debug.WriteLine("{0}{1}", "End: Get()", sw.ElapsedMilliseconds);
            sw.Stop();

            return data;
        }

        [HttpGet]
        [Route("api/values/nocache")]
        public List<TestViewModel> GetNoCache([FromUri] int records)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Debug.WriteLine("Begin: Get()");

            var data = StaticHelper.CreateTestData(records);

            Debug.WriteLine("{0}{1}", "End: Get()", sw.ElapsedMilliseconds);
            sw.Stop();

            return data;
        }

        [HttpGet]
        [Route("api/values/nocache/gzip")]
        [GzipCompress]
        public List<TestViewModel> GetNoCacheGzip([FromUri] int records)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Debug.WriteLine("Begin: Get()");

            var data = StaticHelper.CreateTestData(records);

            Debug.WriteLine("{0}{1}", "End: Get()", sw.ElapsedMilliseconds);
            sw.Stop();

            return data;
        }

        [HttpGet]
        [Route("api/values/cache")]
        public List<TestViewModel> GetCache([FromUri] int records)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Debug.WriteLine("Begin: GetSecond()");

            var data = StaticHelper.GetDataWithMemCaching(records, "testModels2");

            Debug.WriteLine("{0}{1}", "End: GetSecond()", sw.ElapsedMilliseconds);
            sw.Stop();

            return data;
        }

        [HttpGet]
        [Route("api/values/cache/gzip")]
        [GzipCompress]
        public List<TestViewModel> GetCacheGzip([FromUri] int records)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Debug.WriteLine("Begin: GetSecond()");

            var data = StaticHelper.GetDataWithMemCaching(records, "testModels2");

            Debug.WriteLine("{0}{1}", "End: GetSecond()", sw.ElapsedMilliseconds);
            sw.Stop();

            return data;
        }

        [HttpPost]
        [Route("api/values/grid")]
        public IHttpActionResult GetGrid([FromBody] Dictionary<string, object> request)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Debug.WriteLine("Begin: Get()");

            var data = GetDataForGrid("testModels", request);

            Debug.WriteLine("{0}{1}", "End: Get()", sw.ElapsedMilliseconds);
            sw.Stop();

            return Ok(data);
        }

        [HttpPost]
        [Route("api/values/grid/columnFilter")]
        public IHttpActionResult GetGridColumnFilter([FromBody] Dictionary<string, object> request)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Debug.WriteLine("Begin: Get()");

            var data = GetDataForGridColumnFilters("testModels", request);

            Debug.WriteLine("{0}{1}", "End: Get()", sw.ElapsedMilliseconds);
            sw.Stop();

            return Ok(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private ResponseOfT<TestViewModel> GetDataForGrid(string cacheKey, Dictionary<string, object> request)
        {
            // figure out caching model
            var memCacher = new MemoryCacher<List<TestViewModel>>();
            List<TestViewModel> results;
            
            // check cache
            var isCached = memCacher.TryGet(cacheKey, out results);            
            if (isCached)
            {
                // cache exists, perform predicate query on cached list data and return
                return BuildPredicateQuery(request, results);
            }

            // a. cache doesn't exist, perform predicate query on IQueryable DB call
            var dbResults = BuildPredicateQuery(request);
            // b. perform Task.Run on memCacher TryGetAndSet
            Task.Run(() => memCacher.TrySet(cacheKey, () => _dbContext.TestViewModels.ToList()));
            return dbResults;
        }        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private List<object> GetDataForGridColumnFilters(string cacheKey, Dictionary<string, object> request)
        {
            // figure out caching model
            var memCacher = new MemoryCacher<List<TestViewModel>>();
            List<TestViewModel> results;
            
            // check cache
            var isCached = memCacher.TryGet(cacheKey, out results);            
            if (isCached)
            {
                // cache exists, perform predicate query on cached list data and return
                return BuildPredicateQueryColumnFilter(request, results);
            }

            // a. cache doesn't exist, perform predicate query on IQueryable DB call
            var dbResults = BuildPredicateQueryColumnFilter(request);
            // b. perform Task.Run on memCacher TryGetAndSet
            Task.Run(() => memCacher.TrySet(cacheKey, () => _dbContext.TestViewModels.ToList()));
            return dbResults;
        }

        /// <summary>
        /// Gets and returns ResponseOfT VW_BASELINE_DATA for Kendo grid. Supports
        /// server side paging, sorting and filtering.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<object> BuildPredicateQueryColumnFilter(Dictionary<string, object> request, List<TestViewModel> data = null)
        {
            TestViewModel dataObj = new TestViewModel();

            // Build query
            string whereClause = BusinessLogicHelper.GetWhereCondition(request);

            // get filterParams from request
            object filterParam;
            bool ableToParseFilterParam = request.TryGetValue("workspaceFilters", out filterParam);

            // get columnFilter from request
            object columnFilter;
            bool columnFilterProvided = request.TryGetValue("columnFilter", out columnFilter);

            // Begin queryable GET
            var queryableResults = data != null ? data.AsQueryable() : _dbContext.TestViewModels.AsQueryable();

            //Apply Filter parameters 
            if (ableToParseFilterParam && filterParam != null)
            {
                var param = JObject.Parse(filterParam.ToString()).ToObject<FilterParams>();
                if (param != null)
                {
                    queryableResults = dataObj.ApplyFilterParams(queryableResults, param);
                }
            }
            else
            {
                queryableResults = queryableResults.Select(y => y).Where(x => (int)x.Id == -1);
            }

            //apply grid options/grid filters
            queryableResults = queryableResults.Where(whereClause);

            // ensure column filter is provided
            if (!columnFilterProvided) throw new Exception("Bad request: No column filter provided.");

            var columnFilterStr = columnFilter.ToString().Replace("\"", "");

            // get result
            IEnumerable<string> results = dataObj.SelectColumnFilterLinq<TestViewModel, string>(queryableResults, columnFilterStr).Distinct().OrderBy(x => x).AsEnumerable();
            return dataObj.ConvertColumnFilterObjects(columnFilterStr, results);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private ResponseOfT<TestViewModel> BuildPredicateQuery(Dictionary<string, object> request, List<TestViewModel> data = null)
        {
            // Build query
            string order = BusinessLogicHelper.GetOrderBy(request, "Value1 asc");
            int skip = BusinessLogicHelper.GetSkip(request);
            int take = BusinessLogicHelper.GetTake(request);

            string whereClause = BusinessLogicHelper.GetWhereCondition(request, typeof(TestViewModel));
            object filterParam;
            bool parsedFilterParams = request.TryGetValue("workspaceFilters", out filterParam);

            // Begin queryable GET
            var queryableResults = data != null ? data.AsQueryable() : _dbContext.TestViewModels.AsQueryable();

            // Apply Filter parameters 
            if (parsedFilterParams && filterParam != null)
            {
                var param = JObject.Parse(filterParam.ToString()).ToObject<FilterParams>();
                if (param != null)
                {
                    queryableResults = new TestViewModel().ApplyFilterParams(queryableResults, param);
                }
            }
            else
            {
                queryableResults = queryableResults.Select(y => y).Where(x => (int)x.Id == 1);
            }

            // apply grid options/grid filters
            queryableResults = queryableResults.Where(whereClause);
            int count = queryableResults.Count();
            queryableResults = queryableResults.OrderBy(order).Skip(skip).Take(take);

            // Return data           
            return new ResponseOfT<TestViewModel>(queryableResults.ToList(), count);
        }
    }
}