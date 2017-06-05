using Xunit;

namespace ODataParser.Test
{
    public class ValueComparisionsTest
    {
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