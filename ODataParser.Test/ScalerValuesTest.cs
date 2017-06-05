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
            Assert.Equal("name", ScalarValues.PropertyName.Parse(parsable));
        }

        [Theory]
        [InlineData("1")]
        [InlineData(" 1")]
        [InlineData(" 1 ")]
        [InlineData("1 ")]
        public void Parse_positive_int_number(string parsable)
        {
            // ACT
            Assert.Equal("1", ScalarValues.Number.Parse(parsable));
        }

        [Theory]
        [InlineData("-1")]
        [InlineData(" -1")]
        [InlineData(" -1 ")]
        [InlineData("-1 ")]
        public void Parse_negative_int_number(string parsable)
        {
            // ACT
            Assert.Equal("-1", ScalarValues.Number.Parse(parsable));
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
            Assert.Equal(result, ScalarValues.Number_.Parse(parsable).Value);
        }

        [Theory]
        [InlineData(true, "true")]
        [InlineData(false, "false")]
        [InlineData(true, "(true)")]
        [InlineData(true, "( true )")]
        [InlineData(true, "(( true ))")]
        [InlineData(true, "( ( true ) )")]
        public void Evaluate_boolean_expressions_v(bool result, string parsable)
        {
            Assert.Equal(result, (ScalarValues.AnyBooleanConstant.Parse(parsable)).Value);
        }
    }
}