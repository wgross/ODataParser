using Sprache;
using System.Linq.Expressions;

namespace ODataParser
{
    /// <summary>
    /// Parser for scalar values OData understands
    /// </summary>
    public class ScalarValues
    {
        public static Parser<string> PropertyName = Parse.Letter.AtLeastOnce().Text().Token();

        public static Parser<string> Number = (from leading in Parse.Optional(Parse.WhiteSpace)
                                               from op in Parse.Optional(Parse.Char('-').Token())
                                               from number in Parse.Decimal
                                               from trailing in Parse.Optional(Parse.WhiteSpace)
                                               select (op.IsDefined ? "-" : "") + number);

        public static Parser<Expression> SignedInteger = (from leading in Parse.Optional(Parse.WhiteSpace)
                                                          from negative in Parse.Optional(Parse.Char('-').Token())
                                                          from number in Parse.Number
                                                          from trailing in Parse.Optional(Parse.WhiteSpace)
                                                          select ConvertSignedInteger(negative, number));

        private static Expression ConvertSignedInteger(IOption<char> negative, string signedNumber)
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
        public static Parser<Expression> Boolean = from parsedValue in Parse.String("true").Or(Parse.String("false")).Text()
                                                   select Expression.Constant(bool.Parse(parsedValue));

        /// <summary>
        /// All Scalar value packed together for convenience
        /// </summary>
        public static Parser<Expression> All = SignedInteger.Or(TickedString).Or(Boolean);
    }
}