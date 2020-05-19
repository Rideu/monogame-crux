
using Crux;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using static Crux.Simplex;

namespace Crux.Tests
{
    public class SimplexTests
    {
        [TestCase(-1234.0f)]
        [TestCase(-4321.0f)]
        [TestCase(-1.0f)]
        [TestCase(-2.0f)]
        [TestCase(-fPI)]
        [TestCase(-fPI / 2 + fPI)]
        [TestCase(-fPI / 2 + fPI)]
        [TestCase(1234.0f)]
        [TestCase(4321.0f)]
        [TestCase(1.0f)]
        [TestCase(2.0f)]
        [TestCase(fPI)]
        [TestCase(fPI / 2 + fPI)]
        [TestCase(fPI / 2 + fPI)]
        public void SinTest(float x)
        {
            var expected = (Math.Sin(x) * 100);
            var actual = (Sin(x) * 100);
            Assert.AreEqual((int)expected, (int)actual, $"{(int)expected} ({expected}) != {(int)actual} ({actual})");
        }

        [TestCase(-1234.0f)]
        [TestCase(-4321.0f)]
        [TestCase(-1.0f)]
        [TestCase(-2.0f)]
        [TestCase(-fPI)]
        [TestCase(-fPI / 2 + fPI)]
        [TestCase(-fPI / 2 + fPI)]
        [TestCase(1234.0f)]
        [TestCase(4321.0f)]
        [TestCase(1.0f)]
        [TestCase(2.0f)]
        [TestCase(fPI)]
        [TestCase(fPI / 2 + fPI)]
        [TestCase(fPI / 2 + fPI)]
        public void CosTest(float x)
        {
            var expected = (Math.Cos(x) * 100);
            var actual = (Cos(x) * 100);
            Assert.AreEqual((int)expected, (int)actual, $"{(int)expected} ({expected}) != {(int)actual} ({actual})");

        }

    }
}