using Sprache;
using System.Linq.Expressions;

namespace Parser
{
    /// <summary>
    /// Parser for scalar values OData understands
    /// </summary>
    public class ScalarValues
    {
        public static Parser<string> PropertyName = Parse.Letter.AtLeastOnce().Text().Token();

        public static Parser<string> Number = (from leading in Parse.Optional(Parse.WhiteSpace)
                                               from op in Parse.Optional(Parse.Char('-'))
                                               from number in Parse.Decimal.Token()
                                               from trailing in Parse.Optional(Parse.WhiteSpace)
                                               select (op.IsDefined ? "-" : "") + number);

        public static Parser<ConstantExpression> Number_ = from leading in Parse.Optional(Parse.WhiteSpace)
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

        public static Parser<ConstantExpression> SignedInteger = (from leading in Parse.Optional(Parse.WhiteSpace)
                                                                  from negative in Parse.Optional(Parse.Char('-').Token())
                                                                  from number in Parse.Number
                                                                  from trailing in Parse.Optional(Parse.WhiteSpace)
                                                                  select SignedIntegerExpression(negative, number));

        private static ConstantExpression SignedIntegerExpression(IOption<char> negative, string signedNumber)
        {
            // add more here: long/int, negative positive
            return Expression.Constant(int.Parse(signedNumber));
        }

        /// <summary>
        /// A test is surrounded with tiocks ('). The text must not contains these.
        /// </summary>
        public static Parser<Expression> TickedString = (from openingTick in Parse.Char('\'')
                                                         from stringContent in Parse.CharExcept('\'').Many().Text()
                                                         from closingTick in Parse.Char('\'')
                                                         select Expression.Constant(stringContent));

        /// <summary>
        /// A boolean value is simply a constant text value of true or false.
        /// </summary>
        //public static Parser<Expression> Boolean = from parsedValue in Parse.String("true").Or(Parse.String("false")).Text()
        //                                           select Expression.Constant(bool.Parse(parsedValue));

        #region Parse boolean constants: <boolean constant> ::= <true|false>

        private static Parser<ConstantExpression> True = Parse.String("true").Token().Return(Expression.Constant(true));
        private static Parser<ConstantExpression> False = Parse.String("false").Token().Return(Expression.Constant(false));
        private static Parser<ConstantExpression> BooleanConstant => True.XOr(False);

        private static Parser<ConstantExpression> BooleanConstantInParenthesis => from left in Parse.Char('(').Token()
                                                                                  from booleanConst in Parse.Ref(() => AnyBooleanConstant)
                                                                                  from right in Parse.Char(')').Token()
                                                                                  select booleanConst;

        /// <summary>
        /// A constant might in parenthesis or not.
        /// </summary>
        public static Parser<ConstantExpression> AnyBooleanConstant => BooleanConstantInParenthesis.XOr(BooleanConstant); // must be XOR

        #endregion Parse boolean constants: <boolean constant> ::= <true|false>

        /// <summary>
        /// All Scalar value packed together for convenience
        /// </summary>
        public static Parser<Expression> All = SignedInteger.Or(TickedString).Or(AnyBooleanConstant);
    }
}