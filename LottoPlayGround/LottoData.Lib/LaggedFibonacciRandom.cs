using System.Collections.Generic;

namespace LottoData.Lib
{
    public class LaggedFibonacciRandom
    {
        private const int K = 10; // largest "-index"
        private const int J = 7; // other "-index"
        private readonly decimal _m;

        private readonly List<decimal> _history = new List<decimal>();
        private decimal _current;

        public LaggedFibonacciRandom(decimal maxCombinations,decimal[] pastResults)
        {
            _history.AddRange(pastResults);
            _m = maxCombinations;

           

        }
        public decimal Next()
        {
            // (a + b) mod n =
            // [(a mod n) + (b mod n)] mod n
            decimal left = _history[0] % _m;    // [x-big]
            decimal right = _history[K - J] % _m; // [x-other]
            decimal sum = left + right;

            _current = (int)(sum % _m);
            _history.Insert(K + 1, _current);  // anew val at end
            _history.RemoveAt(0);  // [0] val irrelevant now
            return _current;
        }
    }
}