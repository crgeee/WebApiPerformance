using System;
using System.Linq.Expressions;

namespace WebApiPerformance.Helpers
{
    /// <summary>
    /// Helper class to build predicates for where clauses
    /// Source code taken from http://www.albahari.com/nutshell/predicatebuilder.aspx
    /// </summary>
    public static class PredicateBuilder
    {
        /// <summary>
        /// Instantiates an expression to true.  Used for initializing AND statements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>A true Expression</returns>
        public static Expression<Func<T, bool>> True<T>() { return f => true; }

        /// <summary>
        /// Instantiates an expression to false.  Used for initializing OR statements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>A false Expression</returns>
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        /// <summary>
        /// Used to OR two expressions together
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1">The calling Expression</param>
        /// <param name="expr2">The Expression passed as an argument</param>
        /// <returns>An OrElse Expression</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
                                                            Expression<Func<T, bool>> expr2)
        {
            var paramExpr = Expression.Parameter(typeof(T));
            var exprBody = Expression.OrElse(expr1.Body, expr2.Body);
            exprBody = (BinaryExpression)new ParameterReplacer(paramExpr).Visit(exprBody);

            return Expression.Lambda<Func<T, bool>>(exprBody, paramExpr);
        }

        /// <summary>
        /// Used to AND two expressions together
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1">The calling Expression</param>
        /// <param name="expr2">The Expression passed as an argument</param>
        /// <returns>An AndAlso Expression</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
                                                             Expression<Func<T, bool>> expr2)
        {
            var paramExpr = Expression.Parameter(typeof(T));
            var exprBody = Expression.AndAlso(expr1.Body, expr2.Body);
            exprBody = (BinaryExpression)new ParameterReplacer(paramExpr).Visit(exprBody);

            return Expression.Lambda<Func<T, bool>>(exprBody, paramExpr);
        }
    }

    /// <summary>
    /// Class used to replace the parameter portion of an expression to ensure all parameters are named the same
    /// </summary>
    internal class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return base.VisitParameter(_parameter);
        }

        internal ParameterReplacer(ParameterExpression parameter)
        {
            _parameter = parameter;
        }
    }
}