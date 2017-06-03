using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ODataParser.Test
{
    public class MultiplePropertyComparisionTest
    {
        public class Data
        {
            public int Integer { get; set; }
            public string String { get; set; }
            public bool Boolean { get; set; }
        }

        [Theory]
        [InlineData("Integer eq 2 and Boolean eq true")]
        [InlineData("(Integer eq 2) and Boolean eq true")]
        [InlineData("(Integer eq 2) and ( Boolean eq true )")]
        [InlineData("Integer eq 2 and (Boolean eq true)")]
        public void Filter_eq_Integer_and_Boolean(string parsable)
        {
            // ARRANGE
            var data = new List<Data> {
                new Data
                {
                    Integer = 2,
                    Boolean = true
                },
                new Data
                {
                    Integer = 2,
                    Boolean = false
                },
                new Data
                {
                    Integer = 1,
                    Boolean = true
                }
            }.AsQueryable();

            // ACT
            var result = data.Where(new ComparisionExpression<Data>().MakeWhere(parsable));

            // ASSERT
            Assert.Same(data.ElementAt(0), result.Single());
        }

        [Theory]
        [InlineData("Integer eq 2 or Boolean eq true")]
        [InlineData("(Integer eq 2) or Boolean eq true")]
        [InlineData("(Integer eq 2) or ( Boolean eq true )")]
        [InlineData("Integer eq 2 or (Boolean eq true)")]
        public void Filter_eq_Integer_or_Boolean(string parsable)
        {
            // ARRANGE
            var data = new List<Data> {
                new Data
                {
                    Integer = 2,
                    Boolean = true
                },
                new Data
                {
                    Integer = 1,
                    Boolean = false
                },
            }.AsQueryable();

            // ACT
            var result = data.Where(new ComparisionExpression<Data>().MakeWhere(parsable));

            // ASSERT
            Assert.Same(data.ElementAt(0), result.Single());
        }
    }
}