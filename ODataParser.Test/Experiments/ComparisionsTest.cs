using Xunit;

namespace ODataParser.Test.Experiments
{
    public class ComparisionsTest
    {
        [Theory]
        [InlineData(true, "1 eq 1")]
        [InlineData(false, " 1 eq 2")]
        [InlineData(true, "1 lt 2 ")]
        [InlineData(true, "1 ne  2")]
        public void Parse_ComparisionExpression(bool result, string parsable)
        {
            Assert.Equal(result, Comparisions.ComparisionExpression.CallAsFunc<bool>(parsable));
            Assert.Equal(result, Comparisions.AnyComparisionExpression.CallAsFunc<bool>(parsable));
            Assert.Equal(result, Comparisions.Complete.CallAsFunc<bool>(parsable));
        }

        [Theory]
        [InlineData(true, "(1 eq 1)")]
        [InlineData(false, "( 1 eq 2)")]
        [InlineData(true, "(1 lt 2 )")]
        [InlineData(true, " ( 1 ne 2 ) ")]
        public void Parse_ComparisionExpressionInParenthesis(bool result, string parsable)
        {
            Assert.Equal(result, Comparisions.ComparisionExpressionInParenthesis.CallAsFunc<bool>(parsable));
            Assert.Equal(result, Comparisions.AnyComparisionExpression.CallAsFunc<bool>(parsable));
            Assert.Equal(result, Comparisions.Complete.CallAsFunc<bool>(parsable));
        }

        [Theory]
        [InlineData(false, " true ne (1 eq 1)")]
        [InlineData(true, " (1 eq 1) eq ( 2 eq 2) ")]
        [InlineData(true, " (1 eq 1) eq ( 2 eq 2) ne ( true eq false) ")]
        public void Parse_Complete(bool result, string parsable)
        {
            Assert.Equal(result, Comparisions.Complete.CallAsFunc<bool>(parsable));
        }

        [Theory()]
        [InlineData(true, " true eq ( false ne ( 1 eq 1)) ")]
        public void Evaluate_comparsion_recursive(bool result, string parsable)
        {
            Assert.Equal(result, Comparisions.Complete.CallAsFunc<bool>(parsable));
        }
    }
}