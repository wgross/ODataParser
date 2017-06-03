using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Parser.Test
{
    public class StringFunctionsTest
    {
        public class Data
        {
            public int Integer { get; set; }
            public string String { get; set; }
            public bool Boolean { get; set; }
        }

        [Theory]
        [InlineData("startswith(String,'test')")]
        [InlineData("( startswith(String,'test') )")]
        public void Parse_startswith(string parseable)
        {
            // ARRANGE
            var data = new List<Data> {
                new Data{ String = "testvalue" }
            }.AsQueryable();

            // ACT
            var result = data.Where(new WhereClause<Data>().Of(parseable));

            // ASSERT
            Assert.Same(data.ElementAt(0), result.Single());
        }

        [Theory]
        [InlineData("endswith(String,'value')")]
        [InlineData("( endswith(String,'value') )")]
        public void Parse_endswith(string parseable)
        {
            // ARRANGE
            var data = new List<Data> {
                new Data{ String = "testvalue" }
            }.AsQueryable();

            // ACT
            var result = data.Where(new WhereClause<Data>().Of(parseable));

            // ASSERT
            Assert.Same(data.ElementAt(0), result.Single());
        }
    }
}