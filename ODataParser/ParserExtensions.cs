using Sprache;
using System;
using System.Linq.Expressions;

namespace ODataParser
{
    public static class ExpressionParserExtensions
    {
        public static T CallAsFunc<T>(this Parser<Expression> expressionParser, string text)
        {
            return ((Expression<Func<T>>)(expressionParser
                .Select(body => Expression.Lambda<Func<T>>(Expression.ConvertChecked(body, typeof(T))))
                .Parse(text)))
                    .Compile()
                    .Invoke();
        }
    }
}