using Sprache;
using Xunit;

namespace Parser.Test
{
    public class ValuesParserTest
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
    }
}