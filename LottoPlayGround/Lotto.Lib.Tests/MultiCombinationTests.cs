using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lotto.Combinatorics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lotto.Lib.Tests
{
    [TestClass]
    public class MultiCombinationTests
    {
        private ICombination<int> _combination;

        [TestInitialize]
        public void Setup()
        {
            _combination = new MultiCombination<int>(5, 2, Enumerable.Range(1, 50).ToArray(),
                Enumerable.Range(1, 10).ToArray());
        }
        [TestMethod]
        public void FirstTest()
        {
            var expected = new[] {1, 2, 3, 4, 5, 1, 2};
            var actual=_combination.Element(0);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }
        [TestMethod]
        public void RandomTest()
        {
            var expected = new[] { 5, 8, 21, 37, 46, 6, 8 };
            var data= _combination.GetIndexOf(expected, Comparer<int>.Default);
            var actual = _combination.Element(data);

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [TestMethod]
        public void RandomTest2()
        {
            var expected = new[] { 5, 17, 27, 33, 42, 9, 10 };
            var data = _combination.GetIndexOf(expected, Comparer<int>.Default);
            var actual = _combination.Element(data);

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [TestMethod]
        public void RandomTest3()
        {
            var expected = new[] { 5, 17, 27, 33, 42, 9, 10 };
            var actual = _combination.Element(38437469);

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [TestMethod]
        public void RandomTest4()
        {
            var expected = new[] { 3, 14, 17, 37, 39, 5, 10 };
            var actual = _combination.Element(23416684);

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [TestMethod]
        public void RandomTest5()
        {
            var expected = new[] { 7, 12, 13, 34, 35, 1, 3 };
            var actual = _combination.Element(48349846);

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [TestMethod]
        public void ReverseFirstTest()
        {
            long expected = 0;
            //var expected = new[] { 1, 2, 3, 4, 5, 1, 2 };
            var actual = _combination.GetIndexOf(new[] { 1, 2, 3, 4, 5, 1, 2 },Comparer<int>.Default);
            Assert.IsTrue(expected==actual);
        }
    }
}
