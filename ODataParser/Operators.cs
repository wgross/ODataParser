using Sprache;
using System.Linq.Expressions;

namespace ODataParser
{
    public class Operators
    {
        private static Parser<ExpressionType> CompareOperator(string op, ExpressionType opType)
        {
            return Parse.String(op).Token().Return(opType);
        }

        public static Parser<ExpressionType> LessThan = CompareOperator("lt", ExpressionType.LessThan);

        public static Parser<ExpressionType> Equal = CompareOperator("eq", ExpressionType.Equal);

        public static Parser<ExpressionType> ComparisionOperators = LessThan.Or(Equal);

        public static Parser<ExpressionType> And = CompareOperator("and", ExpressionType.And);

        public static Parser<ExpressionType> Or = CompareOperator("or", ExpressionType.Or);

        public static Parser<char> OpeningBrace => from openingBrace in Parse.Char('(')
                                                   from trailingWS in Parse.Optional(Parse.WhiteSpace)
                                                   select openingBrace;

        public static Parser<char> ClosingBrace => from leadingWs in Parse.Optional(Parse.WhiteSpace)
                                                   from closingBrace in Parse.Char(')')
                                                   select closingBrace;
    }
}