using Sprache;
using System.Linq.Expressions;

namespace ODataParser
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

        /// <summary>
        /// All Avaliable value comparision operators
        /// </summary>
        /// <remarks>
        /// XOr isn't possible here: the first char isn't unique in this operator set.
        /// Leading and trailing spaces are removed.
        /// </remarks>
        public static Parser<ExpressionType> ComparisionOperators = LessThan.Or(LessThanOrEqual).Or(Equal).Or(NotEqual).Or(GreaterThan).Or(GreaterThanOrEqual).Token();

        #endregion Value comparision operators

        #region Value arithmethic operators

        public static Parser<ExpressionType> Add = Operator("add", ExpressionType.Add);
        public static Parser<ExpressionType> Sub = Operator("sub", ExpressionType.Subtract);
        public static Parser<ExpressionType> Mul = Operator("mul", ExpressionType.Multiply);
        public static Parser<ExpressionType> Div = Operator("div", ExpressionType.Divide);
        public static Parser<ExpressionType> Pow = Operator("pow", ExpressionType.Power);

        public static Parser<ExpressionType> ArithmeticOperators = Add.XOr(Sub).XOr(Mul).XOr(Div).XOr(Pow).Token();
        public static Parser<ExpressionType> AditiveArithmeticOperators = Add.XOr(Sub).Token();
        public static Parser<ExpressionType> MultiplicativeArithmeticOperators = Mul.XOr(Div).Token();

        #endregion Value arithmethic operators

        #region Boolean operators

        public static Parser<ExpressionType> And = Operator("and", ExpressionType.And);
        public static Parser<ExpressionType> Or = Operator("or", ExpressionType.Or);
        public static Parser<ExpressionType> XOr => Operator("xor", ExpressionType.ExclusiveOr);
        public static Parser<ExpressionType> Not => Operator("not", ExpressionType.Not);
        public static Parser<ExpressionType> BinaryBoolean = And.XOr(Or).XOr(XOr).Token();

        #endregion Boolean operators

        public static Parser<char> OpeningBrace => Parse.Char('(').Token();
        public static Parser<char> ClosingBrace => Parse.Char(')').Token();
        public static Parser<char> Comma => Parse.Char(',').Token();
    }
}