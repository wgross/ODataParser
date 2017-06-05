using Sprache;
using System.Linq.Expressions;
using Xunit;

namespace ODataParser.Test
{
    public class ValueComparisionParserTest
    {
        [Theory]
        [InlineData("lt")]
        [InlineData(" lt")]
        [InlineData(" lt ")]
        [InlineData("lt ")]
        public void Parse_lt(string parsable)
        {
            Assert.Equal(ExpressionType.LessThan, ValueComparisonParser.LessThan.Parse(parsable));
            Assert.Equal(ExpressionType.LessThan, ValueComparisonParser.ComparisionOperators.Parse(parsable));
        }

        [Theory]
        [InlineData("le")]
        [InlineData(" le")]
        [InlineData(" le ")]
        [InlineData("le ")]
        public void Parse_le(string parsable)
        {
            Assert.Equal(ExpressionType.LessThanOrEqual, ValueComparisonParser.LessThanOrEqual.Parse(parsable));
            Assert.Equal(ExpressionType.LessThanOrEqual, ValueComparisonParser.ComparisionOperators.Parse(parsable));
        }

        [Theory]
        [InlineData("eq")]
        [InlineData(" eq")]
        [InlineData(" eq ")]
        [InlineData("eq ")]
        public void Parse_eq(string parsable)
        {
            Assert.Equal(ExpressionType.Equal, ValueComparisonParser.Equal.Parse(parsable));
            Assert.Equal(ExpressionType.Equal, ValueComparisonParser.ComparisionOperators.Parse(parsable));
        }

        [Theory]
        [InlineData("ne")]
        [InlineData(" ne")]
        [InlineData(" ne ")]
        [InlineData("ne ")]
        public void Parse_ne(string parsable)
        {
            Assert.Equal(ExpressionType.NotEqual, ValueComparisonParser.NotEqual.Parse(parsable));
            Assert.Equal(ExpressionType.NotEqual, ValueComparisonParser.ComparisionOperators.Parse(parsable));
        }

        [Theory]
        [InlineData("ge")]
        [InlineData(" ge")]
        [InlineData(" ge ")]
        [InlineData("ge ")]
        public void Parse_ge(string parsable)
        {
            Assert.Equal(ExpressionType.GreaterThanOrEqual, ValueComparisonParser.GreaterThanOrEqual.Parse(parsable));
            Assert.Equal(ExpressionType.GreaterThanOrEqual, ValueComparisonParser.ComparisionOperators.Parse(parsable));
        }

        [Theory]
        [InlineData("gt")]
        [InlineData(" gt")]
        [InlineData(" gt ")]
        [InlineData("gt ")]
        public void Parse_gt(string parsable)
        {
            Assert.Equal(ExpressionType.GreaterThan, ValueComparisonParser.GreaterThan.Parse(parsable));
            Assert.Equal(ExpressionType.GreaterThan, ValueComparisonParser.ComparisionOperators.Parse(parsable));
        }

        [Theory]
        [InlineData(true, "1 eq 1")]
        [InlineData(true, "(1 eq 1)")]
        public void Evaluate_comparsion(bool result, string parsable)
        {
            Assert.Equal(result, ValueComparisonParser.Evaluate(parsable));
        }
    }
}