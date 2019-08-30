using System;
using System.Collections.Generic;
using System.Linq;

namespace Lotto.Combinatorics
{
    public interface IMulitCombination<T> : ICombination<T>
    {
        ICombination<T> Main { get; }
        ICombination<T> Sub { get; }
    }

    public class MultiCombination<T> : IMulitCombination<T>
    {

        public int TotalElements { get; }
        public int ChosenElements { get; }

        public MultiCombination(int mainElements, int subElements, T[] mainEnumerable, T[] subEnumerable, GenerationType type = GenerationType.WithoutRepetitions)
        {
            Main = new Combination<T>(mainElements, mainEnumerable, type);
            Sub = new Combination<T>(subElements, subEnumerable, type);
            TotalElements = mainEnumerable.Length + subEnumerable.Length;
            ChosenElements = mainElements + subElements;
        }

        public decimal GetIndexOf(T[] combinations, IComparer<T> comparer)
        {
            var main = Main.GetIndexOf(combinations.Take(Main.ChosenElements).ToArray(), comparer);
            var sub = Sub.GetIndexOf(combinations.Skip(Main.ChosenElements).ToArray(), comparer);
            return main * Sub.TotalCombinations + sub;
        }

        public T[] Element(decimal m)
        {
            decimal actual = m;
            long main = (long)(actual / Sub.TotalCombinations);
            long sub = (long)actual - main * Sub.TotalCombinations;
            T[] mainElements = Main.Element(main);
            T[] subElements = Sub.Element(sub);
            return mainElements.Concat(subElements).ToArray();
        }

        public long TotalCombinations
        {
            get
            {
                return Main.TotalCombinations * Sub.TotalCombinations;
            }
        }
        public ICombination<T> Main { get; }
        public ICombination<T> Sub { get; }
    }
}