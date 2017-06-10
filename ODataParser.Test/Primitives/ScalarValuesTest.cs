using ODataParser.Primitives;
using Sprache;
using System;
using System.Globalization;
using Xunit;

namespace ODataParser.Test.Primitives
{
    public class ScalerValuesTest
    {
        #region String

        [Theory]
        [InlineData("name")]
        [InlineData(" name")]
        [InlineData("name ")]
        public void Parse_String(string parsable)
        {
            // ACT
            Assert.Equal("name", ScalarValues.String.Parse(parsable));
            //Assert.Equal("name", ScalarValues.PropertyName.EvaluatePredicate<string>(parsable));
        }

        #endregion String

        #region Number

        [Theory]
        [InlineData("1")]
        [InlineData(" 1")]
        [InlineData(" 1 ")]
        [InlineData("1 ")]
        [InlineData("-1")]
        [InlineData(" -1")]
        [InlineData(" -1 ")]
        [InlineData("-1 ")]
        public void Parse_int_Number(string parsable)
        {
            // ACT

            Assert.Equal(int.Parse(parsable), ScalarValues.Number.CallAsFunc<int>(parsable));
            Assert.Equal(long.Parse(parsable), ScalarValues.Number.CallAsFunc<long>(parsable));
            Assert.Equal(float.Parse(parsable, NumberStyles.Number, CultureInfo.InvariantCulture), ScalarValues.Number.CallAsFunc<float>(parsable));
            Assert.Equal(double.Parse(parsable, NumberStyles.Number, CultureInfo.InvariantCulture), ScalarValues.Number.CallAsFunc<double>(parsable));
            Assert.Equal(decimal.Parse(parsable, NumberStyles.Number, CultureInfo.InvariantCulture), ScalarValues.Number.CallAsFunc<decimal>(parsable));
        }

        [Theory]
        [InlineData("1.1")]
        [InlineData(" 1.1")]
        [InlineData(" 1.1 ")]
        [InlineData("1.1 ")]
        [InlineData("-1.1")]
        [InlineData(" -1.1")]
        [InlineData(" -1.1 ")]
        [InlineData("-1.1 ")]
        public void Parse_float_Number(string parsable)
        {
            // ACT

            Assert.Equal(float.Parse(parsable, NumberStyles.Number, CultureInfo.InvariantCulture), ScalarValues.Number.CallAsFunc<float>(parsable));
            //ugly rounding//Assert.Equal(double.Parse(parsable, NumberStyles.Number, CultureInfo.InvariantCulture), ScalarValues.Number.CallAsFunc<double>(parsable));
            Assert.Equal(decimal.Parse(parsable, NumberStyles.Number, CultureInfo.InvariantCulture), ScalarValues.Number.CallAsFunc<decimal>(parsable));
        }

        [Theory]
        [InlineData("1.")]
        [InlineData(" 1.")]
        [InlineData(" 1. ")]
        [InlineData("1. ")]
        [InlineData("-1.")]
        [InlineData(" -1.")]
        [InlineData(" -1. ")]
        [InlineData("-1. ")]
        public void Parse_float_Number_no_decimals(string parsable)
        {
            // ACT

            Assert.Equal(float.Parse(parsable, NumberStyles.Number, CultureInfo.InvariantCulture), ScalarValues.Number.CallAsFunc<float>(parsable));
            //ugly rounding//Assert.Equal(double.Parse(parsable, NumberStyles.Number, CultureInfo.InvariantCulture), ScalarValues.Number.CallAsFunc<double>(parsable));
            Assert.Equal(decimal.Parse(parsable, NumberStyles.Number, CultureInfo.InvariantCulture), ScalarValues.Number.CallAsFunc<decimal>(parsable));
        }

        [Theory]
        [InlineData(".1")]
        [InlineData(" .1")]
        [InlineData(" .1 ")]
        [InlineData(".1 ")]
        [InlineData("-.1")]
        [InlineData(" -.1")]
        [InlineData(" -.1 ")]
        [InlineData("-.1 ")]
        private void Parse_number_fails_on_leading_dot(string parsable)
        {
            // ACT & ASSERT
            Assert.Throws<ParseException>(() => ScalarValues.Number.Parse(parsable));
        }

        [Fact]
        public void Parse_Number_as_long_if_int_is_too_small()
        {
            // ARRANGE

            long intMax_plus_1 = (((long)int.MaxValue) + 1);
            string parsable = intMax_plus_1.ToString();

            // ACT

            Assert.Throws<OverflowException>(() => ScalarValues.Number.CallAsFunc<int>(parsable));
            Assert.Equal(intMax_plus_1, ScalarValues.Number.CallAsFunc<long>(parsable));
            Assert.Equal(intMax_plus_1, ScalarValues.Number.CallAsFunc<decimal>(parsable));
            Assert.Equal(intMax_plus_1, ScalarValues.Number.CallAsFunc<float>(parsable));
            Assert.Equal(intMax_plus_1, ScalarValues.Number.CallAsFunc<double>(parsable));
        }

        [Fact]
        public void Parse_negative_Number_as_long_if_int_is_too_small()
        {
            // ARRANGE

            long intMin_min_1 = (((long)int.MinValue) - 1);
            string parsable = intMin_min_1.ToString();

            // ACT

            Assert.Throws<OverflowException>(() => ScalarValues.Number.CallAsFunc<int>(parsable));
            Assert.Equal(intMin_min_1, ScalarValues.Number.CallAsFunc<long>(parsable));
            Assert.Equal(intMin_min_1, ScalarValues.Number.CallAsFunc<decimal>(parsable));
            Assert.Equal(intMin_min_1, ScalarValues.Number.CallAsFunc<float>(parsable));
            Assert.Equal(intMin_min_1, ScalarValues.Number.CallAsFunc<double>(parsable));
        }

        [Fact]
        public void Parse_Number_as_decimal_if_long_is_too_small()
        {
            // ARRANGE

            decimal longMax_plus_1 = (((decimal)long.MaxValue) + 1);
            string parsable = longMax_plus_1.ToString();

            // ACT

            Assert.Throws<OverflowException>(() => ScalarValues.Number.CallAsFunc<int>(parsable));
            Assert.Throws<OverflowException>(() => ScalarValues.Number.CallAsFunc<long>(parsable));
            Assert.Equal(longMax_plus_1, ScalarValues.Number.CallAsFunc<decimal>(parsable));
        }

        [Fact]
        public void Parse_negative_Number_as_decimal_if_long_is_too_small()
        {
            // ARRANGE

            decimal longMin_min_1 = (((decimal)long.MinValue) - 1);
            string parsable = longMin_min_1.ToString();

            // ACT

            Assert.Throws<OverflowException>(() => ScalarValues.Number.CallAsFunc<int>(parsable));
            Assert.Throws<OverflowException>(() => ScalarValues.Number.CallAsFunc<long>(parsable));
            Assert.Equal(longMin_min_1, ScalarValues.Number.CallAsFunc<decimal>(parsable));
        }

        #endregion Number

        #region BooleanConstant,BooleanConstantInParenthesis,AnyBooleanConstant

        [Theory]
        [InlineData(true, "true")]
        [InlineData(true, " true ")]
        [InlineData(false, "false")]
        [InlineData(false, " false ")]
        public void Evaluate_BooleanContant(bool result, string parsable)
        {
            Assert.Equal(result, ScalarValues.BooleanConstant.CallAsFunc<bool>(parsable));
            Assert.Equal(result, ScalarValues.AnyBooleanConstant.CallAsFunc<bool>(parsable));
        }

        [Theory]
        [InlineData(true, "(true)")]
        [InlineData(true, " (true) ")]
        [InlineData(true, "( true )")]
        public void Evaluate_AnyBooleanContantInParenthesis(bool result, string parsable)
        {
            Assert.Equal(result, ScalarValues.BooleanConstantInParenthesis.CallAsFunc<bool>(parsable));
            Assert.Equal(result, ScalarValues.AnyBooleanConstant.CallAsFunc<bool>(parsable));
        }

        [Theory]
        [InlineData(true, "((true))")]
        [InlineData(false, "(( false ))")]
        [InlineData(true, "( ( true ) )")]
        public void Evaluate_AnyBooleanContantInParenthesis_recursive(bool result, string parsable)
        {
            Assert.Equal(result, ScalarValues.BooleanConstantInParenthesis.CallAsFunc<bool>(parsable));
            Assert.Equal(result, ScalarValues.AnyBooleanConstant.CallAsFunc<bool>(parsable));
        }

        #endregion BooleanConstant,BooleanConstantInParenthesis,AnyBooleanConstant

        #region StringInTicks

        [Theory]
        [InlineData("text", "'text'")]
        [InlineData(" text ", "' text '")]
        [InlineData(" text ", " ' text ' ")]
        public void Evaluate_StringInTicks(string result, string parsable)
        {
            Assert.Equal(result, ScalarValues.StringConstant.CallAsFunc<string>(parsable));
        }

        #endregion StringInTicks
    }
}