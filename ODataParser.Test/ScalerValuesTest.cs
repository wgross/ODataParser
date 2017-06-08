using ODataParser.Test;
using Sprache;
using Xunit;

namespace Parser.Test
{
    public class ScalerValuesTest
    {
        #region String

        [Theory]
        [InlineData("name")]
        [InlineData(" name")]
        [InlineData("name ")]
        public void Parse_String(string parsable)
        {
            // ACT
            Assert.Equal("name", ScalarValues.String.Parse(parsable));
            //Assert.Equal("name", ScalarValues.PropertyName.EvaluatePredicate<string>(parsable));
        }

        #endregion String

        #region Number

        [Theory]
        [InlineData(1, "1")]
        [InlineData(1, " 1")]
        [InlineData(1, " 1 ")]
        [InlineData(1, "1 ")]
        [InlineData(-1, "-1")]
        [InlineData(-1, " -1")]
        [InlineData(-1, " -1 ")]
        [InlineData(-1, "-1 ")]
        public void Parse_int_Number(int result, string parsable)
        {
            // ACT
            Assert.Equal(result, ScalarValues.Number.CallAsFunc<int>(parsable));
        }

        #endregion Number

        #region BooleanConstant,BooleanConstantInParenthesis,AnyBooleanConstant

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

        #endregion BooleanConstant,BooleanConstantInParenthesis,AnyBooleanConstant

        #region StringInTicks

        [Theory]
        [InlineData("text", "'text'")]
        [InlineData(" text ", "' text '")]
        [InlineData(" text ", " ' text ' ")]
        public void Evaluate_StringInTicks(string result, string parsable)
        {
            Assert.Equal(result, ScalarValues.StringConstant.CallAsFunc<string>(parsable));
        }

        #endregion StringInTicks
    }
}