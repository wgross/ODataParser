using Sprache;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace ODataParser
{
    public class FilterInterpreter
    {
        public IQueryable<T> Filter<T>(IQueryable<T> source, string expression)
        {
            var parameter = Expression.Parameter(typeof(T));
            return source.Where(
                Expression.Lambda<Func<T, bool>>(
                    body: Expression.LessThan(
                        parameter,
                        PromoteContant(typeof(T), ScalarValues.Number.Parse(expression))),
                    parameters: parameter));
        }

        private Expression PromoteContant(Type type, string constant)
        {
            if (type == typeof(int))
            {
                return Expression.Constant(int.Parse(ScalarValues.Number.Parse(constant)));
            }
            throw new InvalidOperationException();
        }
    }
}