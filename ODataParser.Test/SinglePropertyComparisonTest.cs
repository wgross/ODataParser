﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ODataParser.Test
{
    public class SinglePropertyComparisonTest
    {
        public class Data
        {
            public int Integer { get; set; }
            public string String { get; set; }
            public bool Boolean { get; set; }
            public DateTimeOffset DateTime { get; set; }
        }

        [Fact]
        public void Filter_eq_Integer()
        {
            // ARRANGE
            var data = new List<Data> {
                new Data{ Integer = 2 }
            }.AsQueryable();

            // ACT
            var result = data.Where("Integer eq 2");

            // ASSERT
            Assert.Same(data.ElementAt(0), result.Single());
        }

        [Fact]
        public void Filter_eq_DateTime()
        {
            // ARRANGE
            var now = DateTimeOffset.Parse("2017-06-16T18:43:07.733+02:00");
            var data = new List<Data> {
                new Data{ DateTime = now }
            }.AsQueryable();

            // ACT
            var result = data.Where($"DateTime eq 2017-06-16T18:43:07.733+02:00");

            // ASSERT
            Assert.Same(data.ElementAt(0), result.Single());
        }

        [Fact]
        public void Filter_eq_String()
        {
            // ARRANGE
            var data = new List<Data> {
                new Data{ String = "test" }
            }.AsQueryable();

            // ACT
            var result = data.Where("String eq 'test'");

            // ASSERT
            Assert.Same(data.ElementAt(0), result.Single());
        }

        [Fact]
        public void Filter_eq_Bool()
        {
            // ARRANGE
            var data = new List<Data> {
                new Data{ Boolean = true }
            }.AsQueryable();

            // ACT
            var result = data.Where("Boolean eq true");

            // ASSERT
            Assert.Same(data.ElementAt(0), result.Single());
        }

        [Theory]
        [InlineData("(Boolean eq true)")]
        [InlineData("(true eq Boolean)")]
        [InlineData("( Boolean eq true)")]
        [InlineData("( Boolean eq true )")]
        [InlineData("( (Boolean eq true) )")]
        public void Filter_ignores_braces_for_compare(string toParse)
        {
            // ARRANGE
            var data = new List<Data> {
                new Data{ Boolean = true }
            }.AsQueryable();

            // ACT
            var result = data.Where(toParse);

            // ASSERT
            Assert.Same(data.ElementAt(0), result.Single());
        }
    }
}