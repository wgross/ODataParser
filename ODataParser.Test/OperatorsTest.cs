﻿using Sprache;
using System.Linq.Expressions;
using Xunit;

namespace Parser.Test
{
    public class OperatorsTest
    {
        [Theory]
        [InlineData("lt")]
        [InlineData(" lt")]
        [InlineData(" lt ")]
        [InlineData("lt ")]
        public void Parse_lt(string parsable)
        {
            Assert.Equal(ExpressionType.LessThan, Operators.LessThan.Parse(parsable));
            Assert.Equal(ExpressionType.LessThan, Operators.ComparisionOperators.Parse(parsable));
        }

        [Theory]
        [InlineData("le")]
        [InlineData(" le")]
        [InlineData(" le ")]
        [InlineData("le ")]
        public void Parse_le(string parsable)
        {
            Assert.Equal(ExpressionType.LessThanOrEqual, Operators.LessThanOrEqual.Parse(parsable));
            Assert.Equal(ExpressionType.LessThanOrEqual, Operators.ComparisionOperators.Parse(parsable));
        }

        [Theory]
        [InlineData("eq")]
        [InlineData(" eq")]
        [InlineData(" eq ")]
        [InlineData("eq ")]
        public void Parse_eq(string parsable)
        {
            Assert.Equal(ExpressionType.Equal, Operators.Equal.Parse(parsable));
            Assert.Equal(ExpressionType.Equal, Operators.ComparisionOperators.Parse(parsable));
        }

        [Theory]
        [InlineData("ne")]
        [InlineData(" ne")]
        [InlineData(" ne ")]
        [InlineData("ne ")]
        public void Parse_ne(string parsable)
        {
            Assert.Equal(ExpressionType.NotEqual, Operators.NotEqual.Parse(parsable));
            Assert.Equal(ExpressionType.NotEqual, Operators.ComparisionOperators.Parse(parsable));
        }

        [Theory]
        [InlineData("ge")]
        [InlineData(" ge")]
        [InlineData(" ge ")]
        [InlineData("ge ")]
        public void Parse_ge(string parsable)
        {
            Assert.Equal(ExpressionType.GreaterThanOrEqual, Operators.GreaterThanOrEqual.Parse(parsable));
            Assert.Equal(ExpressionType.GreaterThanOrEqual, Operators.ComparisionOperators.Parse(parsable));
        }

        [Theory]
        [InlineData("gt")]
        [InlineData(" gt")]
        [InlineData(" gt ")]
        [InlineData("gt ")]
        public void Parse_gt(string parsable)
        {
            Assert.Equal(ExpressionType.GreaterThan, Operators.GreaterThan.Parse(parsable));
            Assert.Equal(ExpressionType.GreaterThan, Operators.ComparisionOperators.Parse(parsable));
        }

        [Theory]
        [InlineData("and")]
        [InlineData(" and")]
        [InlineData(" and ")]
        public void Parse_and(string parsable)
        {
            Assert.Equal(ExpressionType.And, Operators.And.Parse(parsable));
            Assert.Equal(ExpressionType.And, Operators.BinaryBoolean.Parse(parsable));
        }

        [Theory]
        [InlineData("or")]
        [InlineData(" or")]
        [InlineData(" or ")]
        public void Parse_or(string parsable)
        {
            Assert.Equal(ExpressionType.Or, Operators.Or.Parse(parsable));
            Assert.Equal(ExpressionType.Or, Operators.BinaryBoolean.Parse(parsable));
        }

        [Theory]
        [InlineData("xor")]
        [InlineData(" xor")]
        [InlineData(" xor ")]
        public void Parse_xor(string parsable)
        {
            Assert.Equal(ExpressionType.ExclusiveOr, Operators.XOr.Parse(parsable));
            Assert.Equal(ExpressionType.ExclusiveOr, Operators.BinaryBoolean.Parse(parsable));
        }

        [Theory]
        [InlineData("not")]
        [InlineData(" not")]
        [InlineData(" not ")]
        public void Parse_not(string parsable)
        {
            Assert.Equal(ExpressionType.Not, Operators.Not.Parse(parsable));
        }
    }
}