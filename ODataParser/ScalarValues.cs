using Sprache;
using System.Linq.Expressions;

namespace Parser
{
    /// <summary>
    /// Parser for scalar values OData understands
    /// </summary>
    public class ScalarValues
    {
        public static readonly Parser<string> String = Parse.Letter.AtLeastOnce().Text().Token();

        #region Parse JSONish number

        public static Parser<ConstantExpression> Number = from leading in Parse.Optional(Parse.WhiteSpace)
                                                          from negative in Parse.Optional(Parse.Char('-'))
                                                          from number in Parse.Decimal.Token()
                                                          from trailing in Parse.Optional(Parse.WhiteSpace)
                                                          select SignedNumberExpression(negative, number);

        private static ConstantExpression SignedNumberExpression(IOption<char> negative, string signedNumber)
        {
            if (negative.IsDefined)
                return Expression.Constant(int.Parse("-" + signedNumber));
            else
                return Expression.Constant(int.Parse(signedNumber));
        }

        #endregion Parse JSONish number

        /// <summary>
        /// A test is surrounded with tiocks ('). The text must not contains these.
        /// </summary>
        public static readonly Parser<ConstantExpression> StringInTicks = from openingTick in Parse.Char('\'')
                                                                          from stringContent in Parse.CharExcept('\'').Many().Text()
                                                                          from closingTick in Parse.Char('\'')
                                                                          select Expression.Constant(stringContent);

        #region Parse boolean constants: <boolean constant> ::= <true|false>

        private static readonly Parser<ConstantExpression> True = Parse.String("true").Token().Return(Expression.Constant(true));
        private static readonly Parser<ConstantExpression> False = Parse.String("false").Token().Return(Expression.Constant(false));

        // Xor is possible because the leading and trailing whitespaces are removed before.
        // afterwards the first char is significant
        public static Parser<ConstantExpression> BooleanConstant => True.XOr(False).Token();

        public static Parser<ConstantExpression> BooleanConstantInParenthesis => from left in Parse.Char('(').Token()
                                                                                 from booleanConst in BooleanConstant.XOr(BooleanConstantInParenthesis).Token()
                                                                                 from right in Parse.Char(')').Token()
                                                                                 select booleanConst;

        /// <summary>
        /// A constant might in parenthesis or not.
        /// </summary>
        /// <remarks>
        /// A boolean constant might be on parenthesis for itself or not.
        /// </remarks>
        public static Parser<ConstantExpression> AnyBooleanConstant => BooleanConstant.XOr(BooleanConstantInParenthesis).Token(); // must be XOR

        #endregion Parse boolean constants: <boolean constant> ::= <true|false>

        /// <summary>
        /// All Scalar value packed together for convenience
        /// </summary>
        public static Parser<Expression> All = Number.Or(StringInTicks).Or(AnyBooleanConstant);
    }
}