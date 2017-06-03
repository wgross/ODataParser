using Sprache;

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

        /// <summary>
        /// A test is enclodes in ' chars. Text ist self may not contain these.
        /// </summary>
        public static Parser<string> Text = (from openingTick in Parse.Char('\'')
                                             from content in Parse.CharExcept('\'').Many().Text()
                                             from closingTick in Parse.Char('\'')
                                             select content).Token();

        public static Parser<string> Boolean = Parse.String("true").Or(Parse.String("false")).Text();

        /// <summary>
        /// All Scalar value packed together for convenience
        /// </summary>
        public static Parser<string> ContantOfAnyType = Number.Or(Text).Or(Boolean);
    }
}