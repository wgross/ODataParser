using Sprache;
using System.Linq.Expressions;
using Xunit;

namespace ODataParser.Test
{
    public class ComparisionOperatorTest
    {
        [Theory]
        [InlineData("lt")]
        [InlineData(" lt")]
        [InlineData(" lt ")]
        [InlineData("lt ")]
        public void Parse_lt(string parseable)
        {
            // ACT

            Assert.Equal(ExpressionType.LessThan, Operators.LessThan.Parse(parseable));
        }
    }
}