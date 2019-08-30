using System.Collections.Generic;

namespace Lotto.Combinatorics
{
    public interface ICombination<T>
    {
        int TotalElements { get; }
        int ChosenElements { get; }
        decimal GetIndexOf(T[] combinations,IComparer<T> comparer);
        T[] Element(decimal m);
        long TotalCombinations { get; }
    }
}