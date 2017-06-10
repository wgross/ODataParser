using Sprache;
using System.Globalization;
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
                                                          from number in Parse.Optional(Parse.Number)
                                                          from decimals in Parse.Optional(from dot in Parse.Char('.')
                                                                                          from decimals in Parse.Number
                                                                                          select decimals)
                                                          from trailing in Parse.Optional(Parse.WhiteSpace)
                                                          select SignedNumberExpression(negative, number, decimals);

        private static ConstantExpression SignedNumberExpression(IOption<char> negative, IOption<string> number, IOption<string> decimals)
        {
            if (decimals.IsDefined)
            {
                var str = $"{(negative.IsDefined ? "-" : string.Empty)}{(number.IsDefined ? number.Get() : "0")}.{decimals.Get()}";
                if (float.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out var floatvalue))
                    return Expression.Constant(floatvalue);
                else if (double.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out var doublevalue))
                    return Expression.Constant(doublevalue);
                return Expression.Constant(decimal.Parse(str, NumberStyles.Number, CultureInfo.InvariantCulture));
            }
            else
            {
                var str = negative.IsDefined ? "-" + number.Get() : number.Get();
                if (int.TryParse(str, out var intValue))
                    return Expression.Constant(intValue);
                else if (long.TryParse(str, out var longValue))
                    return Expression.Constant(longValue);
                else return Expression.Constant(decimal.Parse(str));
            }
        }

        #endregion Parse JSONish number

        #region Parse string constants: '<text>'

        /// <summary>
        /// A string contant is surreounded by ticke ('). Leading and trailing spaces are ignored outside of teh ticked area.
        /// </summary>
        public static readonly Parser<ConstantExpression> StringConstant = (from openingTick in Parse.Char('\'')
                                                                            from stringContent in Parse.CharExcept('\'').Many().Text()
                                                                            from closingTick in Parse.Char('\'')
                                                                            select Expression.Constant(stringContent)).Token();

        #endregion Parse string constants: '<text>'

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
        public static Parser<Expression> All = Number.Or(StringConstant).Or(AnyBooleanConstant);
    }
}