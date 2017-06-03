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
        public void Parse_lt(string parsable)
        {
            // ACT

            Assert.Equal(ExpressionType.LessThan, Operators.LessThan.Parse(parsable));
        }

        [Theory]
        [InlineData("and")]
        [InlineData(" and")]
        [InlineData(" and ")]
        public void Parse_and(string parsable)
        {
            Assert.Equal(ExpressionType.And, Operators.And.Parse(parsable));
        }

        [Theory]
        [InlineData("or")]
        [InlineData(" or")]
        [InlineData(" or ")]
        public void Parse_or(string parsable)
        {
            Assert.Equal(ExpressionType.Or, Operators.Or.Parse(parsable));
        }
    }
}