using System;
using System.Collections.Generic;
using System.Linq;

namespace Lotto.Combinatorics
{
    public static class SharedFunctions
    {
       
        public static IEnumerable<IEnumerable<T>> Combinatorics<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(
              emptyProduct,
              (accumulator, sequence) =>
                from accseq in accumulator
                from item in sequence
                select accseq.Concat(new[] { item }));
        }
        public static decimal LargestV(decimal a, decimal b, decimal x)
        {
            decimal v = a - 1;

            while (Choose(v, b) > x)
                --v;

            return v;
        } // LargestV()
        public static decimal Choose(decimal n, decimal k, GenerationType type = GenerationType.WithoutRepetitions)
        {
            if (type == GenerationType.WithoutRepetitions)
            {
                if (n < 0 || k < 0)
                    throw new Exception("Invalid negative parameter in Choose()");
                if (n < k)
                    return 0;  // special case
                if (n == k)
                    return 1;

                decimal delta, iMax;

                if (k < n - k) // ex: Choose(100,3)
                {
                    delta = n - k;
                    iMax = k;
                }
                else         // ex: Choose(100,97)
                {
                    delta = k;
                    iMax = n - k;
                }

                decimal ans = delta + 1;

                for (long i = 2; i <= iMax; ++i)
                {
                    checked { ans = (ans * (delta + i)) / i; }
                }
                return ans;

            }
            else
            {
                if (n < 0 || k < 0)
                    throw new Exception("Invalid negative parameter in Choose()");
                return Convert.ToDecimal(Math.Pow((long)n, (long)k));
            }



        } // Choose()



    }

}
