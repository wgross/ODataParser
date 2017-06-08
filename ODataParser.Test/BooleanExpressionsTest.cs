using Sprache;
using Xunit;

namespace ODataParser.Test
{
    public class BooleanExpressionsTest
    {
        [Theory]
        [InlineData(true, "not false")]
        [InlineData(false, "not true")]
        [InlineData(false, " not true ")]
        [InlineData(true, "not not true")]
        [InlineData(false, "not true and true")]
        public void Parse_UnaryBooleanExpression(bool result, string parsable)
        {
            Assert.Equal(result, BooleanExpressions.UnaryBooleanExpression.CallAsFunc<bool>(parsable));
            Assert.Equal(result, BooleanExpressions.BooleanExpression.CallAsFunc<bool>(parsable));
            Assert.Equal(result, BooleanExpressions.AnyBooleanExpression.CallAsFunc<bool>(parsable));
            Assert.Equal(result, BooleanExpressions.Complete.CallAsFunc<bool>(parsable));
        }

        [Theory]
        [InlineData(true, " false or true")]
        [InlineData(false, "false and true ")]
        [InlineData(true, " false xor   true ")]
        public void Parse_BinaryBooleanExpression(bool result, string parsable)
        {
            Assert.Equal(result, BooleanExpressions.BinaryBooleanExpression.CallAsFunc<bool>(parsable));
            Assert.Equal(result, BooleanExpressions.BooleanExpression.CallAsFunc<bool>(parsable));
            Assert.Equal(result, BooleanExpressions.AnyBooleanExpression.CallAsFunc<bool>(parsable));
            Assert.Equal(result, BooleanExpressions.Complete.CallAsFunc<bool>(parsable));
        }

        [Theory]
        [InlineData(true, "(not false)")]
        [InlineData(false, " ( not true )")]
        [InlineData(false, " ( ( not true ) ) ")]
        [InlineData(true, "( not (( not true )))")]
        [InlineData(true, "(false or true)")]
        [InlineData(false, " ( false and true )")]
        [InlineData(true, " ( ( false xor   true ) ) ")]
        public void Parse_BooleanExpressionInParenthesis(bool result, string parsable)
        {
            Assert.Equal(result, BooleanExpressions.BooleanExpressionInParenthesis.CallAsFunc<bool>(parsable));
            Assert.Equal(result, BooleanExpressions.AnyBooleanExpression.CallAsFunc<bool>(parsable));
            Assert.Equal(result, BooleanExpressions.Complete.CallAsFunc<bool>(parsable));
        }

        [Theory]
        [InlineData(true, "(not true) or true")]
        [InlineData(true, "not true xor true")]
        [InlineData(false, "not true and true")]
        [InlineData(false, "(not (true or ((true and false) and false)))")]
        [InlineData(false, "false and (true or false)")]
        [InlineData(false, " (true or false) xor true")]
        public void Parse_Complete(bool result, string parsable)
        {
            Assert.Equal(result, BooleanExpressions.Complete.CallAsFunc<bool>(parsable));
        }

        [Theory]
        [InlineData(true, "true or ( false and (true or false) )")]
        [InlineData(false, "not true or ( false and (true or false) )")]
        public void Parse_Complete_recursive(bool result, string parsable)
        {
            Assert.Equal(result, BooleanExpressions.Complete.CallAsFunc<bool>(parsable));
        }

        [Theory]
        [InlineData("true or ( (true or false) and false )")]
        [InlineData("not ( true or ( (true or false) and false ))")]
        public void DontParse_bacuse_of_unknown_reason(string parseable)
        {
            Assert.Throws<ParseException>(() => BooleanExpressions.Complete.CallAsFunc<bool>(parseable));
        }
    }
}