using System;
using LottoData.Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lotto.Lib.Tests
{
    [TestClass]
    public class RandomTests
    {
        private SecureRandom _random;

        [TestInitialize]
        public void Setup()
        {
            _random = new SecureRandom();
        }
        [TestMethod]
        public void DoubleTest()
        {
            var nextD = _random.NextDouble();
            Assert.AreNotEqual(0.0, nextD);
        }
        [TestMethod]
        public void IntInRangeTest()
        {
            var nextD = _random.Next(0,10);
            Assert.AreNotEqual(0.0, nextD);
        }
    }
}
