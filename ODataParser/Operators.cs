using Sprache;
using System.Linq.Expressions;

namespace Parser
{
    public class Operators
    {
        private static Parser<ExpressionType> Operator(string op, ExpressionType opType) => Parse.String(op).Token().Return(opType);

        #region Value comparision operators

        public static Parser<ExpressionType> LessThan = Operator("lt", ExpressionType.LessThan);
        public static Parser<ExpressionType> LessThanOrEqual = Operator("le", ExpressionType.LessThanOrEqual);
        public static Parser<ExpressionType> Equal = Operator("eq", ExpressionType.Equal);
        public static Parser<ExpressionType> NotEqual = Operator("ne", ExpressionType.NotEqual);
        public static Parser<ExpressionType> GreaterThanOrEqual = Operator("ge", ExpressionType.GreaterThanOrEqual);
        public static Parser<ExpressionType> GreaterThan = Operator("gt", ExpressionType.GreaterThan);

        public static Parser<ExpressionType> ComparisionOperators = LessThan.Or(LessThanOrEqual).Or(Equal).Or(NotEqual).Or(GreaterThan).Or(GreaterThanOrEqual);

        #endregion Value comparision operators

        #region Boolean operators

        public static Parser<ExpressionType> And = Operator("and", ExpressionType.And);
        public static Parser<ExpressionType> Or = Operator("or", ExpressionType.Or);
        public static Parser<ExpressionType> XOr => Operator("xor", ExpressionType.ExclusiveOr);
        public static Parser<ExpressionType> Not => Operator("not", ExpressionType.Not);


        #endregion Boolean operators

        public static Parser<char> OpeningBrace => Parse.Char('(').Token();
        public static Parser<char> ClosingBrace => Parse.Char(')').Token();
        public static Parser<char> Comma => Parse.Char(',').Token();
    }
}