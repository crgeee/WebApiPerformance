using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using WebApiPerformance.Helpers;

namespace WebApiPerformance.Models
{
    public abstract class BaseViewModel
    {
        public virtual int Id { get; set; }
        public virtual string Value1 { get; set; }

        #region FUNCTIONS

        /// <summary>
        /// Generic function for applying filter parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="filterParams"></param>
        /// <returns></returns>
        public abstract IQueryable<T> ApplyFilterParams<T>(IQueryable<T> data, FilterParams filterParams) where T : BaseViewModel;

        /// <summary>
        /// Converts List of strings to a list of Ints in a safe manner.
        /// </summary>
        /// <param name="listOfInts"></param>
        /// <returns>List<int/></returns>
        /// functin created to address Bug:1596
        protected virtual List<int> ParseIntList(List<string> listOfInts)
        {
            List<int> retVal = new List<int>();
            var regex = new Regex("[a-zA-Z]");
            bool containsString = false;
            for (int index = 0; index < listOfInts.Count; index++)
            {
                var el = listOfInts[index];
                if (regex.IsMatch(el))
                {
                    containsString = true;
                    break;
                }
            }

            if (!containsString)
            {
                retVal = listOfInts.Select(Int32.Parse).ToList();
            }
            return retVal;
        }

        /// <summary>
        /// Converts Column Filter results into Kendo column filter object that is serializable.
        /// </summary>
        /// <param name="columnFilter"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        public virtual List<object> ConvertColumnFilterObjects(string columnFilter, IEnumerable<string> results)
        {
            return (from item in results
                    where item != null
                    select new Dictionary<string, object> { { columnFilter, item } }).Cast<object>().ToList();
        }

        /// <summary>
        /// create linq selector
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="input"></param>
        /// <param name="property"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private static IQueryable<TResult> CreateSelector<TInput, TResult>(IQueryable<TInput> input, MemberInfo property, QueryableMonad<TInput, TResult> method)
        {
            var source = Expression.Parameter(typeof(TInput), "x");
            Expression propertyAccessor = Expression.MakeMemberAccess(source, property);
            var expression = Expression.Lambda<Func<TInput, TResult>>(propertyAccessor, source);
            return method(input, expression);
        }

        /// <summary>
        /// TODO?
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="input"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        private delegate IQueryable<TResult> QueryableMonad<TInput, TResult>(IQueryable<TInput> input, Expression<Func<TInput, TResult>> mapper);

        /// <summary>
        /// create column filter
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="input"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public IQueryable<TResult> SelectColumnFilterLinq<TInput, TResult>(IQueryable<TInput> input, string propertyName)
        {
            var property = typeof(TInput).GetProperty(propertyName);
            return CreateSelector<TInput, TResult>(input, property, Queryable.Select);
        }

        #endregion FUNCTIONS

        #region PREDICATES

        /// <summary>
        /// Creates ACFP_ID predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <param name="predicate"></param>
        /// <returns>Expression&lt;Func&lt;T, bool&gt;&gt;</returns>
        protected virtual Expression<Func<T, bool>> IdPredicate<T>(List<int> ids, Expression<Func<T, bool>> predicate) where T : BaseViewModel
        {
            // ACFP_ID
            if (ids.Count > 0)
            {
                predicate = predicate.And(x => ids.Contains((int)x.Id));
            }
            return predicate;
        }

        /// <summary>
        /// Create STRIKE_STATUS predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value1s"></param>
        /// <param name="predicate"></param>
        /// <returns>Expression&lt;Func&lt;T, bool&gt;&gt;</returns>
        protected virtual Expression<Func<T, bool>> Value1Predicate<T>(List<string> value1s, Expression<Func<T, bool>> predicate) where T : BaseViewModel
        {
            // STRIKE_STATUS
            if (value1s.Count > 0)
            {
                predicate = predicate.And(x => value1s.Contains(x.Value1));
            }
            return predicate;
        }

        #endregion PREDICATES
    }
}