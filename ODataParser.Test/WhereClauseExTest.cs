using Sprache;
using System.Linq.Expressions;
using Xunit;

namespace ODataParser.Test
{
    public class WhereClauseExTest
    {
        private class Data
        {
            public int Integer { get; internal set; }
            public string String { get; internal set; }
        }

        #region Expressions resolve to numbers

        [Theory]
        [InlineData((int)1, "1")]
        [InlineData((float)1.0, "1.0")]
        public void Parse_constant(object result, string parsable)
        {
            Assert.Equal(result, new WhereClauseEx<Data>().Constant.Parse(parsable).Value);
            Assert.Equal(result, ((ConstantExpression)new WhereClauseEx<Data>().Factor.Parse(parsable)).Value);
            Assert.Equal(result, ((ConstantExpression)new WhereClauseEx<Data>().Operand.Parse(parsable)).Value);
        }

        [Theory]
        [InlineData(6, "2 mul 3")]
        [InlineData(1, "2.0 mul 0.5")]
        public void Parse_multipicative(int result, string parsable)
        {
            Assert.Equal(result, new WhereClauseEx<Data>().MutiplicativeTerm.CallAsFunc<int>(parsable));
            Assert.Equal(result, new WhereClauseEx<Data>().AdditiveTerm.CallAsFunc<int>(parsable));
        }

        [Theory]
        [InlineData(5, "2 add 3")]
        [InlineData(7, "1 add 2 mul 3")] // mul first
        public void Parse_additive(int result, string parsable)
        {
            Assert.Equal(result, new WhereClauseEx<Data>().AdditiveTerm.CallAsFunc<int>(parsable));
        }

        #endregion Expressions resolve to numbers

        #region Expressions resolve to text

        [Theory]
        [InlineData("text", "'text'")]
        [InlineData("", "''")]
        public void Parse_constant_text(string result, string parsable)
        {
            Assert.Equal(result, new WhereClauseEx<Data>().Constant.Parse(parsable).Value);
            Assert.Equal(result, ((ConstantExpression)new WhereClauseEx<Data>().Factor.Parse(parsable)).Value);
            Assert.Equal(result, ((ConstantExpression)new WhereClauseEx<Data>().Operand.Parse(parsable)).Value);
        }

        #endregion Expressions resolve to text

        #region Expressions resolve to bool

        [Theory]
        [InlineData(true, "true")]
        [InlineData(false, "false")]
        public void Parse_boolean_constant(bool result, string parsable)
        {
            Assert.Equal(result, new WhereClauseEx<Data>().ComparativeTerm.CallAsFunc<bool>(parsable));
            Assert.Equal(result, new WhereClauseEx<Data>().BooleanTerm.CallAsFunc<bool>(parsable));
            Assert.Equal(result, new WhereClauseEx<Data>().Of(parsable).Compile().Invoke(new Data()));
        }

        [Theory]
        [InlineData(true, "1 eq 1")]
        [InlineData(true, "1.1 eq 1.1")]
        [InlineData(true, "1 ne 2")]
        [InlineData(true, "5 eq 2 add 3")]
        [InlineData(true, "7 eq 1 add 2 mul 3")]
        [InlineData(true, "true and 7 eq 1 add 2 mul 3")]
        [InlineData(false, "false and 7 eq 1 add 2 mul 3")]
        public void Parse_comparative_number(bool result, string parsable)
        {
            Assert.Equal(result, new WhereClauseEx<Data>().ComparativeTerm.CallAsFunc<bool>(parsable));
            Assert.Equal(result, new WhereClauseEx<Data>().BooleanTerm.CallAsFunc<bool>(parsable));
            Assert.Equal(result, new WhereClauseEx<Data>().Of(parsable).Compile().Invoke(new Data()));
        }

        [Theory]
        [InlineData(true, "'text' ne 'test'")]
        public void Parse_comparative_texts(bool result, string parsable)
        {
            Assert.Equal(result, new WhereClauseEx<Data>().ComparativeTerm.CallAsFunc<bool>(parsable));
            Assert.Equal(result, new WhereClauseEx<Data>().BooleanTerm.CallAsFunc<bool>(parsable));
            Assert.Equal(result, new WhereClauseEx<Data>().Of(parsable).Compile().Invoke(new Data()));
        }

        [Theory]
        [InlineData(true, "'test' eq 'test'")]
        [InlineData(false, "'test' eq 'TEST'")]
        public void Parse_comparative_string(bool result, string parsable)
        {
            Assert.Equal(result, new WhereClauseEx<Data>().ComparativeTerm.CallAsFunc<bool>(parsable));
            Assert.Equal(result, new WhereClauseEx<Data>().BooleanTerm.CallAsFunc<bool>(parsable));
            Assert.Equal(result, new WhereClauseEx<Data>().Of(parsable).Compile().Invoke(new Data()));
        }

        [Theory]
        [InlineData(true, "1 eq 1 and 1 ne 2")]
        [InlineData(true, "1 eq 1 or 1 ne 1")]
        [InlineData(true, "1 eq 1")]
        [InlineData(true, "1 ne 2")]
        [InlineData(true, "5 eq 2 add 3")]
        [InlineData(true, "7 eq 1 add 2 mul 3")]
        public void Parse_boolean(bool result, string parsable)
        {
            Assert.Equal(result, new WhereClauseEx<Data>().BooleanTerm.CallAsFunc<bool>(parsable));
            Assert.Equal(result, new WhereClauseEx<Data>().Of(parsable).Compile().Invoke(new Data()));
        }

        [Theory]
        [InlineData(1, "Integer")]
        public void Parse_int_property(object result, string parsable)
        {
            var data = new Data
            {
                Integer = 1
            };

            Assert.Equal(result, new WhereClauseEx<Data>().GetProperty<int>(parsable).Compile().Invoke(data));
        }

        [Theory]
        [InlineData(true, "2 ne Integer")]
        [InlineData(true, "Integer eq Integer")]
        public void Parse_property_expression(bool result, string parsable)
        {
            var data = new Data
            {
                Integer = 1
            };

            Assert.Equal(result, new WhereClauseEx<Data>().Of(parsable).Compile().Invoke(data));
        }

        [Theory]
        [InlineData(true, "startswith(String,'begin')")]
        [InlineData(true, "endswith(String,'end')")]
        public void Parse_function_expression(bool result, string parsable)
        {
            var data = new Data
            {
                String = "begin_end"
            };

            Assert.Equal(result, new WhereClauseEx<Data>().Of(parsable).Compile().Invoke(data));
        }

        #endregion Expressions resolve to bool
    }
}