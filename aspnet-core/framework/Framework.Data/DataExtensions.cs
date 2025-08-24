using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;


namespace Framework.Data
{
    internal static class DataExtensions
    {
        ///// <summary>
        ///// Returns the first element in a sequence.
        ///// </summary>
        ///// <typeparam name="TSource">The type of the elements of source.</typeparam>
        ///// <param name="tableQuery">A TableQuery{TSource} to return the first element of</param>
        //public static TSource First<TSource>(this TableQuery<TSource> tableQuery) where TSource : ITableEntity
        //{
        //    return ((IEnumerable<TSource>)tableQuery.Take(1)).First();
        //}

        ///// <summary>
        ///// Returns the first element in a sequence that satisfies a specified condition.
        ///// </summary>
        ///// <typeparam name="TSource">The type of the elements of source.</typeparam>
        ///// <param name="tableQuery">A TableQuery{TSource} to return the first element of</param>
        ///// <param name="predicate">A function to test an element for a condition.</param>
        //public static TSource First<TSource>(this TableQuery<TSource> tableQuery, Expression<Func<TSource, bool>> predicate) where TSource : ITableEntity
        //{
        //    return tableQuery.Where(predicate).Take(1).First();
        //}

        ///// <summary>
        ///// Returns the first element of the sequence or a default value if no such element is found.
        ///// </summary>
        ///// <typeparam name="TSource">The type of the elements of source.</typeparam>
        ///// <param name="tableQuery">A TableQuery{TSource} to return the first element of</param>
        //public static TSource FirstOrDefault<TSource>(this TableQuery<TSource> tableQuery) where TSource : ITableEntity
        //{
        //    return ((IEnumerable<TSource>)tableQuery.Take(1)).FirstOrDefault();
        //}

        ///// <summary>
        ///// Returns the first element of the sequence that satisfies a condition or a default value if no such element is found.
        ///// </summary>
        ///// <typeparam name="TSource">The type of the elements of source.</typeparam>
        ///// <param name="tableQuery">A TableQuery{TSource} to return the first element of</param>
        ///// <param name="predicate">A function to test an element for a condition.</param>
        //public static TSource FirstOrDefault<TSource>(this TableQuery<TSource> tableQuery, Expression<Func<TSource, bool>> predicate) where TSource : ITableEntity
        //{
        //    return tableQuery.Where(predicate).Take(1).FirstOrDefault();
        //}

        ///// <summary>
        ///// Return the only element of a sequence, and throws an exception if there is no exactly one element in the sequence.
        ///// </summary>
        ///// <typeparam name="TSource">The type of the elements of source.</typeparam>
        ///// <param name="tableQuery">A TableQuery{TSource}> to return the single element of</param>
        ///// <param name="predicate">A function to test an element for a condition.</param>
        //public static TSource Single<TSource>(this TableQuery<TSource> tableQuery, Expression<Func<TSource, bool>> predicate) where TSource : ITableEntity
        //{
        //    // Get 2 and try to get single ^^
        //    return tableQuery.Where(predicate).Take(2).Single();
        //}

        ///// <summary>
        ///// Returns the only element of a sequence, or a default value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.
        ///// </summary>
        ///// <typeparam name="TSource">The type of the elements of source.</typeparam>
        ///// <param name="tableQuery">A TableQuery{TSource}> to return the single element of</param>
        ///// <param name="predicate">A function to test an element for a condition.</param>
        //public static TSource SingleOrDefault<TSource>(this TableQuery<TSource> tableQuery, Expression<Func<TSource, bool>> predicate) where TSource : ITableEntity
        //{
        //    // Get 2 and try to get single ^^
        //    return tableQuery.Where(predicate).Take(2).SingleOrDefault();
        //}

        public static Expression<Func<TEntity, bool>> Contains<TEntity, TProperty>(this IEnumerable<object> values, Expression<Func<TEntity, TProperty>> expression)
        {
            // Get the property name
            var propertyName = ((PropertyInfo)((MemberExpression)expression.Body).Member).Name;

            // Create the parameter expression
            var parameterExpression = Expression.Parameter(typeof(TEntity), "e");

            // Init the body
            Expression mainBody = Expression.Constant(false);

            foreach (var value in values)
            {
                // Create the equality expression
                var equalityExpression = Expression.Equal(
                    Expression.PropertyOrField(parameterExpression, propertyName),
                    Expression.Constant(value));

                // Add to the main body
                mainBody = Expression.OrElse(mainBody, equalityExpression);
            }

            return Expression.Lambda<Func<TEntity, bool>>(mainBody, parameterExpression);
        }
    }
}
