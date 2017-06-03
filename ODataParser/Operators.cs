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
    }
}