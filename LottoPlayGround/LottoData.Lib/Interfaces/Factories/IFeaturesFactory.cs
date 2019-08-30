using System.Collections.Generic;
using Lotto.Combinatorics;
using LottoData.Lib.DataTypes;
using LottoData.Lib.Interfaces.DataTypes;

namespace LottoData.Lib.Interfaces.Factories
{
    public interface IFeaturesFactory
    {
        int SumOfBalls(IData row);
        int VortexNumber(decimal number);
        int VortexCircleLevel(int[] balls, ICombination<int> model);
        int VortexDegree(int[] balls, ICombination<int> model);
        int GetMax(long maxCom);
        long[] GetIntArray(decimal num);
        long CombIndex(int[] ballsArray,ICombination<int> model);
        List<IData> ToFibIndexs(BaseData<int>[] toArray, ICombination<int> model);
        List<IData> FibIndexs(long index);
        string FibonomialNumber(long number);
        List<IData> ToBase9(IEnumerable<IData> balls);
        IData Base9Data(IData data);
        List<IData> ToFibIndexs(BaseData<int> indexData);
        bool IsVortex(int absDiff, int vortexValue);
        decimal FibBinToDec(string fibBin);
        IEnumerable<IData> GetPossiblePredictions(IEnumerable<string> fibBins, ICombination<int> model);
        List<IData> ToPatternMatches(BaseData<int>[] toArray, ICombination<int> model);
        IData CreateDraw(int[] drawBalls, ICombination<int> model);
        List<IData> SumIndexes(int factor, int sum, int baseSum);
    }
}