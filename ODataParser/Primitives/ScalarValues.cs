using Sprache;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace ODataParser.Primitives
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
                                                          from number in Parse.Number
                                                          from decimals in Parse.Optional(from dot in Parse.Char('.')
                                                                                          from decimals in Parse.Number
                                                                                          select decimals)
                                                          from trailing in Parse.Optional(Parse.WhiteSpace)
                                                          select SignedNumberExpression(negative, number, decimals);

        private static ConstantExpression SignedNumberExpression(IOption<char> negative, string number, IOption<string> decimals)
        {
            if (decimals.IsDefined)
            {
                var str = $"{(negative.IsDefined ? "-" : string.Empty)}{(number)}.{decimals.Get()}";
                if (float.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out var floatvalue))
                    return Expression.Constant(floatvalue);
                else if (double.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out var doublevalue))
                    return Expression.Constant(doublevalue);
                return Expression.Constant(decimal.Parse(str, NumberStyles.Number, CultureInfo.InvariantCulture));
            }
            else
            {
                var str = $"{(negative.IsDefined ? "-" : string.Empty)}{number}";
                if (int.TryParse(str, out var intValue))
                    return Expression.Constant(intValue);
                else if (long.TryParse(str, out var longValue))
                    return Expression.Constant(longValue);
                else return Expression.Constant(decimal.Parse(str));
            }
        }

        #endregion Parse JSONish number

        #region Parse Dates and times

        public static Parser<ConstantExpression> DateTimeOffset => from leading in Parse.Optional(Parse.WhiteSpace)
                                                                   from dateTime in ScalarValues.DateTime
                                                                   from offset in Parse.Optional(ScalarValues.TimezoneOffset)
                                                                   from trailing in Parse.Optional(Parse.WhiteSpace)
                                                                   select Expression.Constant(new DateTimeOffset(dateTime, offset.GetOrElse(TimeSpan.Zero)));

        private static Parser<DateTime> DateTime = from leading in Parse.Optional(Parse.WhiteSpace)
                                                   from date in ScalarValues.Date
                                                   from T in Parse.Char('T')
                                                   from time in ScalarValues.Time
                                                   from msec in Parse.Optional(from dot in Parse.Char('.')
                                                                               from msec in Parse.Number
                                                                               select int.Parse(msec))
                                                   select new DateTime(
                                                       year: date.ElementAt(0),
                                                       month: date.ElementAt(1),
                                                       day: date.ElementAt(2),
                                                       hour: time.ElementAt(0),
                                                       minute: time.ElementAt(1),
                                                       second: time.ElementAt(2),
                                                       millisecond: msec.GetOrElse(0));

        private static Parser<IEnumerable<int>> Date = Parse.DelimitedBy(parser: Parse.Number.Select(txt => int.Parse(txt)), delimiter: Parse.Char('-'));

        private static Parser<IEnumerable<int>> Time = Parse.DelimitedBy(parser: Parse.Number.Select(txt => int.Parse(txt)), delimiter: Parse.Char(':'));

        private static Parser<TimeSpan> TimezoneOffset = from tzSep in Parse.Chars('+', '-', 'Z')
                                                         from hourOffset in Parse.Number.Select(str => int.Parse(str))
                                                         from offsetSep in Parse.Char(':')
                                                         from minOffset in Parse.Number.Select(str => int.Parse(str))
                                                         select tzSep == '+'
                                                         ? new TimeSpan(hourOffset, minOffset, seconds: 0)
                                                         : TimeSpan.Zero.Subtract(new TimeSpan(hourOffset, minOffset, seconds: 0));

        #endregion Parse Dates and times

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