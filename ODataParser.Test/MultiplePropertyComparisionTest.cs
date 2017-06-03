using System;
using System.Collections.Generic;
using System.Text;

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

        [Fact]
        public void Filter_eq_Integer()
        {
            // ARRANGE
            var data = new List<Data> {
                new Data{ Integer = 2 }
            }.AsQueryable();

            // ACT
            var result = data.Where(new ComparisionExpression<Data>().MakeWhere("Integer eq 2"));

            // ASSERT
            Assert.Same(data.ElementAt(0), result.Single());
        }

    }
}
