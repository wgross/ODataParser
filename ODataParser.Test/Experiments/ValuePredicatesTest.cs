﻿using Xunit;

namespace ODataParser.Test.Experiments
{
    public class ValuePredicatesTest
    {
        //[Theory]
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
            Assert.Equal(result, new ValuePredicates().Evaluate(parsable));
        }

        //[Theory]
        //[InlineData(true, "not false")]
        [InlineData(true, "not (1 eq 2)")]
        //[InlineData(false, "not true")]
        //[InlineData(false, "not ( true )")]
        //[InlineData(false, "( not ( true ))")]
        //[InlineData(true, "not ( not true)")]
        //[InlineData(false, "not ( not ( not true))")]
        //[InlineData(false, "not (true or false)")]
        //[InlineData(false, "not ( (true or false))")]
        public void Evaluate_unary_boolean_expressions_v(bool result, string parsable)
        {
            Assert.Equal(result, new ValuePredicates().Evaluate(parsable));
        }

        //[Theory()]
        [InlineData(true, "(1 eq 1) and (2 gt 1)")]
        public void Evaluate_predicate(bool result, string parsable)
        {
            Assert.Equal(result, new ValuePredicates().Evaluate(parsable));
        }
    }
}