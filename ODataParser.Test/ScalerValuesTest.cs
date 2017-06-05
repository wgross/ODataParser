using ODataParser.Test;
using Sprache;
using Xunit;

namespace Parser.Test
{
    public class ScalerValuesTest
    {
        [Theory]
        [InlineData("name")]
        [InlineData(" name")]
        [InlineData("name ")]
        public void Parser_recognizes_name(string parsable)
        {
            // ACT
            Assert.Equal("name", ScalarValues.String.Parse(parsable));
            //Assert.Equal("name", ScalarValues.PropertyName.EvaluatePredicate<string>(parsable));
        }

        [Theory]
        [InlineData(1, "1")]
        [InlineData(1, " 1")]
        [InlineData(1, " 1 ")]
        [InlineData(1, "1 ")]
        [InlineData(-1, "-1")]
        [InlineData(-1, " -1")]
        [InlineData(-1, " -1 ")]
        [InlineData(-1, "-1 ")]
        public void Parse_int_number(int result, string parsable)
        {
            // ACT
            Assert.Equal(result, ScalarValues.Number.CallAsFunc<int>(parsable));
        }

        [Theory]
        [InlineData(true, "true")]
        [InlineData(true, " true ")]
        [InlineData(false, "false")]
        [InlineData(false, " false ")]
        public void Evaluate_BooleanContant(bool result, string parsable)
        {
            Assert.Equal(result, ScalarValues.BooleanConstant.CallAsFunc<bool>(parsable));
            Assert.Equal(result, ScalarValues.AnyBooleanConstant.CallAsFunc<bool>(parsable));
        }

        [Theory]
        [InlineData(true, "(true)")]
        [InlineData(true, " (true) ")]
        [InlineData(true, "( true )")]
        public void Evaluate_AnyBooleanContantInParenthesis(bool result, string parsable)
        {
            Assert.Equal(result, ScalarValues.BooleanConstantInParenthesis.CallAsFunc<bool>(parsable));
            Assert.Equal(result, ScalarValues.AnyBooleanConstant.CallAsFunc<bool>(parsable));
        }

        [Theory]
        [InlineData(true, "((true))")]
        [InlineData(false, "(( false ))")]
        [InlineData(true, "( ( true ) )")]
        public void Evaluate_AnyBooleanContantInParenthesis_recursive(bool result, string parsable)
        {
            Assert.Equal(result, ScalarValues.BooleanConstantInParenthesis.CallAsFunc<bool>(parsable));
            Assert.Equal(result, ScalarValues.AnyBooleanConstant.CallAsFunc<bool>(parsable));
        }

        [Theory]
        [InlineData("text", "'text'")]
        [InlineData(" text ", "' text '")]
        public void Evaluate_string_in_ticks_expression(string result, string parsable)
        {
            Assert.Equal(result, ScalarValues.StringInTicks.CallAsFunc<string>(parsable));
        }
    }
}