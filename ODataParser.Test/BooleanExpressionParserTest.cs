﻿using Xunit;

namespace ODataParser.Test
{
    public class BooleanExpressionParserTest
    {
        [Theory]
        [InlineData(true, "true")]
        [InlineData(false, "false")]
        [InlineData(true, "(true)")]
        [InlineData(true, "( true )")]
        [InlineData(true, "(( true ))")]
        [InlineData(true, "( ( true ) )")]
        public void Evaluate_boolean_expressions_v(bool result, string parsable)
        {
            //Assert.Equal(result, BooleanExpressionParser.EvaluateConstant(parsable));
            Assert.Equal(result, BooleanExpressionParser.Evaluate(parsable));
        }

        //[Fact]
        //public void Evaluate_boolean_expressions_dbg()
        //{
        //    Assert.Equal(true, BooleanExpressionParser2.EvaluateValue("true and true"));
        //}

        //[Theory]
        //[InlineData(true, "true and true")]
        //[InlineData(true, "(true and true)")]
        //[InlineData(false, "true and false")]
        //[InlineData(true, "true or false")]
        //[InlineData(false, "false or false")]
        //[InlineData(true, "true xor false")]
        //[InlineData(false, "true xor true")]
        //public void Evaluate_binary_boolean_expressions(bool result, string parsable)
        //{
        //    Assert.Equal(result, BooleanExpressionParser.EvaluateExpresssion(parsable));
        //}

        [Theory]
        [InlineData(true, "true and true")]
        [InlineData(true, "(true and true)")]
        [InlineData(false, "true and false")]
        [InlineData(true, "true or false")]
        [InlineData(false, "false or false")]
        [InlineData(true, "true xor false")]
        [InlineData(false, "true xor true")]
        [InlineData(true, "true and (true or false)")]
        [InlineData(true, "(false and true) or (true or false)")]
        public void Evaluate_binary_boolean_expressions_v(bool result, string parsable)
        {
            Assert.Equal(result, BooleanExpressionParser.Evaluate(parsable));
        }

        //[Theory]
        //[InlineData(true, "not false")]
        //[InlineData(false, "not true")]
        //[InlineData(false, "not ( true )")]
        //[InlineData(false, "( not ( true ))")]
        //[InlineData(false, "not (true or false)")]
        //public void Evaluate_unary_boolean_expressions(bool result, string parsable)
        //{
        //    Assert.Equal(result, BooleanExpressionParser.EvaluateExpresssion(parsable));
        //}

        [Theory]
        [InlineData(true, "not false")]
        [InlineData(false, "not true")]
        [InlineData(false, "not ( true )")]
        [InlineData(false, "( not ( true ))")]
        [InlineData(true, "not ( not true)")]
        [InlineData(false, "not ( not ( not true))")]
        [InlineData(false, "not (true or false)")]
        [InlineData(false, "not ( (true or false))")]
        public void Evaluate_unary_boolean_expressions_v(bool result, string parsable)
        {
            Assert.Equal(result, BooleanExpressionParser.Evaluate(parsable));
        }

        //[Theory]
        //[InlineData(false, "not ((true eq true) or (true eq false))")]
        //[InlineData(false, "not (true or (true eq false))")]
        //public void Evaluate_complex_boolean_expressions_v(bool result, string parsable)
        //{
        //    Assert.Equal(result, BooleanExpressionParser2.EvaluateValue(parsable));
        //}
    }
}