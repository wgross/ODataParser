using Xunit;

namespace ODataParser.Test
{
    public class ValueComparisionsTest
    {
        [Theory]
        [InlineData(true, "1 eq 1")]
        [InlineData(false, " 1 eq 2")]
        [InlineData(true, "1 lt 2 ")]
        [InlineData(true, "1 ne  2")]
        public void Parse_ComparisionExpression(bool result, string parsable)
        {
            Assert.Equal(result, ValueComparisons.ComparisionExpression.CallAsFunc<bool>(parsable));
            Assert.Equal(result, ValueComparisons.AnyComparisionExpression.CallAsFunc<bool>(parsable));
        }

        [Theory]
        [InlineData(true, "(1 eq 1)")]
        [InlineData(false, "( 1 eq 2)")]
        [InlineData(true, "(1 lt 2 )")]
        [InlineData(true, " ( 1 ne 2 ) ")]
        public void Parse_ComparisionExpressionInParenthesis(bool result, string parsable)
        {
            Assert.Equal(result, ValueComparisons.ComparisionExpressionInParenthesis.CallAsFunc<bool>(parsable));
            Assert.Equal(result, ValueComparisons.AnyComparisionExpression.CallAsFunc<bool>(parsable));
        }

        [Theory]
        [InlineData(true, "1 eq 1")]
        [InlineData(true, "(1 eq 1)")]
        [InlineData(true, "true eq true")]
        public void Evaluate_comparsion(bool result, string parsable)
        {
            Assert.Equal(result, ValueComparisons.Evaluate(parsable));
        }

        [Theory()]
        [InlineData(true, "true eq (2 gt 1)")]
        [InlineData(true, "(1 eq 1) eq (2 gt 1)")]
        public void Evaluate_comparsion_recursive(bool result, string parsable)
        {
            Assert.Equal(result, ValueComparisons.Evaluate(parsable));
        }
    }
}