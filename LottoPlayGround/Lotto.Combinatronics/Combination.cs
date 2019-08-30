using System;
using System.Collections.Generic;
using System.Linq;

namespace Lotto.Combinatorics
{
    public class Combination<T> : ICombination<T>
    {
        public int TotalElements { get; }
        public int ChosenElements { get; }

        private readonly GenerationType _combinationType;
        private readonly T[] _elements;
        public Combination(int k, T[] elements, GenerationType type = GenerationType.WithoutRepetitions)
        {
            _combinationType = type;
            if (elements.Count() < 0 || k < 0) // normally TotalElements >= ChoosenElements
                throw new Exception("Negative parameter in constructor");
            _elements = elements;
            TotalElements = elements.Count();
            ChosenElements = k;

        }

        public decimal GetIndexOf(T[] combinations,IComparer<T> comparer)
        {

            decimal sum = 0;
            if (_combinationType == GenerationType.WithoutRepetitions)
            {
                sum = SharedFunctions.Choose(TotalElements, ChosenElements);
                for (int i = 0; i < combinations.Count(); i++)
                {
                    sum = sum - SharedFunctions.Choose(_elements.Count() - (_elements.ToList().IndexOf(combinations[i]) + 1), ChosenElements - i);
                }
            }
            else
            {
                SortedSet<T> list= new SortedSet<T>(_elements, comparer);
                List<T> asList = list.ToList();
                T max = asList.Last();
                long maxValue = asList.IndexOf(max) + 1;
                for (int i = 0; i < combinations.Count(); i++)
                {
                    long currIndex = asList.IndexOf(combinations[i]);
                    sum = sum + (currIndex) * SharedFunctions.Choose(maxValue, ChosenElements - (i + 1), _combinationType);
                }
                sum = sum + 1;
            }

            return sum - 1;
        }
        public T[] Element(decimal m)
        {
            if (m > SharedFunctions.Choose(TotalElements, ChosenElements, _combinationType))
                throw new Exception("Index is greater than total elememts");
            T[] currElement = new T[ChosenElements];
            int[] ans = new int[ChosenElements];

            decimal a = TotalElements;
            decimal b = ChosenElements;
            if (_combinationType == GenerationType.WithoutRepetitions)
            {
                decimal x = (SharedFunctions.Choose(TotalElements, ChosenElements) - 1) - m; // x is the "dual" of m

                for (int i = 0; i < ChosenElements; ++i)
                {
                    ans[i] = (int)SharedFunctions.LargestV(a, b, x); // largest value v, where v < a and vCb < x    
                    x = x - SharedFunctions.Choose(ans[i], b);
                    a = ans[i];
                    b = b - 1;
                }

                for (int i = 0; i < ChosenElements; ++i)
                {
                    ans[i] = (TotalElements - 1) - ans[i];
                    currElement[i] = _elements[ans[i]];
                }
            }
            else
            {
                decimal number = m;
                long i = ChosenElements - 1;

                while (i >= 0)
                {
                    var index = (int)(number % TotalElements);
                    number = number / TotalElements;
                    currElement[i] = _elements[index];
                    i--;
                }
            }


            return currElement;

        }

        public long TotalCombinations
        {
            get
            {
                return (long)SharedFunctions.Choose(TotalElements,ChosenElements, _combinationType);
            }
        }
    }

    
}
