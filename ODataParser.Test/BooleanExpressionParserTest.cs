using Xunit;

namespace ODataParser.Test
{
    public class BooleanExpressionParserTest
    {
        [Theory]
        [InlineData(true, "true")]
        [InlineData(false, "false")]
        [InlineData(true, "(true)")]
        [InlineData(true, "( true )")]
        [InlineData(true, "(( true ))")]
        public void Evaluate_boolean_expressions(bool result, string parsable)
        {
            Assert.Equal(result, BooleanExpressionParser.EvaluateConstant(parsable));
            Assert.Equal(result, BooleanExpressionParser.EvaluateValue(parsable));
        }

        [Theory]
        [InlineData(true, "true and true")]
        [InlineData(false, "true and false")]
        [InlineData(true, "true or false")]
        [InlineData(false, "false or false")]
        [InlineData(true, "true xor false")]
        [InlineData(false, "true xor true")]
        public void Evaluate_binary_boolean_expressions(bool result, string parsable)
        {
            Assert.Equal(result, BooleanExpressionParser.EvaluateExpresssion(parsable));
        }

        [Theory]
        [InlineData(true, "true and true")]
        [InlineData(false, "true and false")]
        [InlineData(true, "true or false")]
        [InlineData(false, "false or false")]
        [InlineData(true, "true xor false")]
        [InlineData(false, "true xor true")]
        public void Evaluate_binary_boolean_expressions_v(bool result, string parsable)
        {
            Assert.Equal(result, BooleanExpressionParser.EvaluateValue(parsable));
        }

        [Theory]
        [InlineData(true, "not false")]
        [InlineData(false, "not true")]
        [InlineData(false, "not ( true )")]
        [InlineData(false, "( not ( true ))")]
        public void Evaluate_unary_boolean_expressions(bool result, string parsable)
        {
            Assert.Equal(result, BooleanExpressionParser.EvaluateExpresssion(parsable));
        }
    }
}