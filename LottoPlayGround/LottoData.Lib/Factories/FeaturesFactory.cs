using System;
using System.Collections.Generic;
using System.Linq;
using Lotto.Combinatorics;
using LottoData.Lib.DataTypes;
using LottoData.Lib.Interfaces.DataTypes;
using LottoData.Lib.Interfaces.Factories;

namespace LottoData.Lib.Factories
{
    public class FeaturesFactory : IFeaturesFactory
    {
        public int SumOfBalls(IData row)
        {
            return ((IDraw)row).BallsArray.Sum();
        }
        public int VortexNumber(decimal number)
        {

            while (number > 9)
            {
                number = GetIntArray(number).Sum();

            }

            return (int)number;
        }

        public int VortexCircleLevel(int[] balls, ICombination<int> model)
        {
            long index = (long)model.GetIndexOf(balls, Comparer<int>.Default);

            long circle = index / GetMax(model.TotalCombinations);


            return (int)circle;
        }
        public int VortexDegree(int[] balls, ICombination<int> model)
        {
            long index = (long)model.GetIndexOf(balls, Comparer<int>.Default);
            long circle = model.TotalCombinations < 360 ? index / 9 : index / 360;


            return (int)(index - (circle * GetMax(model.TotalCombinations)));
        }
        public int GetMax(long maxCom)
        {
            int i = maxCom < 360 ? 9 : 360;
            return i;
        }
        public long[] GetIntArray(decimal num)
        {
            List<long> listOfInts = new List<long>();
            while (num > 0)
            {
                listOfInts.Add((long)num % 10);
                num = num / 10;
            }
            listOfInts.Reverse();
            return listOfInts.ToArray();
        }
        public long CombIndex(int[] ballsArray,ICombination<int> model)
        {
            return (long) model.GetIndexOf(ballsArray, Comparer<int>.Default);
        }

        public List<IData> ToFibIndexs(BaseData<int>[] toArray, ICombination<int> model)
        {
            long index = (long)model.GetIndexOf(toArray.Select(x => x.Data).ToArray(), Comparer<int>.Default);
            return FibIndexs(index);
      }

        public List<IData> FibIndexs(long index)
        {
            List<long> fibos= new List<long>(FibonomialNumbers(index));
            long fibIndex = fibos.Count!=0?fibos.Max():0;
            string fibString = FibonomialNumber(index);
            int fibZero = fibString.ToCharArray().Count(c => c == '0');
            int fibOne = fibString.ToCharArray().Count(c => c == '1');
            return new List<IData>
            {
                new FeatureData<long>(FeatureNames.Fibo, fibIndex),
                new FeatureData<long>(FeatureNames.FiboDiff, Math.Abs(fibIndex - index)),
                new FeatureData<long>(FeatureNames.FiboDiffVortex, VortexNumber(Math.Abs((int) (fibIndex - index)))),
               // new FeatureData<long>(FeatureNames.Fib2BinDec, Convert.ToInt32(fibString, 2)),
                new FeatureData<string>(FeatureNames.FiboBin,"\""+string.Join("-",fibString.ToCharArray())+ "\""),
                new FeatureData<string>(FeatureNames.FibColl, string.Join(",",fibos)),
                new FeatureData<long>(FeatureNames.FiboZeros,fibZero ),
                new FeatureData<long>(FeatureNames.FiboOnes, fibOne)
            };
        }

        private List<long> fibo = new List<long> { 1, 2, 3, 5, 8, 13, 21 };
        public string FibonomialNumber(long number)
        {
            while (fibo[fibo.Count - 1] < number)
            {
                fibo.Add(fibo[fibo.Count - 1] + fibo[fibo.Count - 2]);
            }
            long given = number;
            int index = fibo.Count - 1;
            int j = 0;
            string result = string.Empty;
            while (given > 0 && j <= index)
            {
                var quotient = given / fibo[index - j];
                if (quotient > 0)
                {

                    given = given - fibo[index - j];
                }
                j++;
                result = result + quotient;
            }

            return result + string.Join("", Enumerable.Range(1, fibo.Count - result.Length).Select(x => '0'));
        }
        public IEnumerable<long> FibonomialNumbers(long number)
        {
            while (fibo[fibo.Count - 1] < number)
            {
                fibo.Add(fibo[fibo.Count - 1] + fibo[fibo.Count - 2]);
            }
            long given = number;
            int index = fibo.Count - 1;
            int j = 0;
            List<long> results=new List<long>();
            while (given > 0 && j <= index)
            {
                var quotient = given / fibo[index - j];
                if (quotient > 0)
                {
                    results.Add(fibo[index - j]);
                    given = given - fibo[index - j];
                }
                j++;
                
            }

            return results;
        }

        public decimal FibBinToDec(string fibBin)
        {
            string withOutQuotes = fibBin.Replace("\"", "").Replace("-", "");
            List<char> charArray = withOutQuotes.ToCharArray().ToList();
            while (charArray.Count>fibo.Count)
            {
                fibo.Add(fibo[fibo.Count - 1] + fibo[fibo.Count - 2]);
            }

            decimal number = 0;
            for (int i = 0; i < charArray.Count; i++)
            {
                number = number + Int32.Parse(charArray[i]+"") * fibo[charArray.Count-1 - i];
            }

            return number;

        }

        public IEnumerable<IData> GetPossiblePredictions(IEnumerable<string> fibBins,ICombination<int> model)
        {
            List<decimal> predictions= new List<decimal>();
            List<string> fibBinList=new List<string>(fibBins.Select(x=>x.Replace("-","")));
            List<string> genFibBinList=new List<string>();
            List<IEnumerable<int>> predictionDraws = new List<IEnumerable<int>>();

            List<IData> draws= new List<IData>();

            int k = fibBinList.Count - 1;
            int j = 0;
            bool forward = true;
            while (k >= 0)
            {
                if (k < fibBinList[0].Length)
                {
                    break;
                }
                int p = 0;
                string first = string.Empty;
                for (int i = k; i >= 0; i--)
                {
                    first = first + fibBinList[i][j];
                    if (forward)
                    {
                        j++;
                        if (j == fibBinList[i].Length)
                        {
                            break;
                        }
                    }
                  
                    else
                    {
                        j--;
                        if (j == -1) break;
                    }

                    p++;
                }
                k = k - p;
                if (forward)
                {
                   j--;
                }
                else
                {
                    j = 0;
                }

                forward = !forward;
                decimal result = FibBinToDec(first);
                if (result < model.TotalCombinations)
                {
                    
                    predictions.Add(result);
                    genFibBinList.Add(first);
                    predictionDraws.Add(model.Element(result));

                    draws.Add(CreateDraw(model.Element(result).ToList(),model));
                }
                
            }
           return draws;
        }

        public List<IData> ToPatternMatches(BaseData<int>[] toArray, ICombination<int> model)
        {
            decimal index = model.GetIndexOf(toArray.Select(x => x.Data).ToArray(), Comparer<int>.Default);
            int[] actual = model.Element(index);
            Dictionary<string, string> patterns =
                new Dictionary<string, string>
                {
                    {FeatureNames.FiboPattern, GetFiboPattern(actual)},
                    {FeatureNames.PrimePattern, GetPrimePattern(actual)},
                    {FeatureNames.PrimeCount, GetCount(GetPrimePattern(actual),model)},
                    {FeatureNames.FiboCount, GetCount(GetFiboPattern(actual),model)}
                };

            return patterns.Select(x => new FeatureData<string>(x.Key, x.Value) as IData).ToList();
        }

        public IData CreateDraw(int[] drawBalls,ICombination<int> model)
        {
            DateTime drawDate= DateTime.Now;
            List<IData> balls = new List<IData>
            {
                new BaseData<DateTime>(drawDate,"Date")
            };
            for (int i = 0; i < drawBalls.Length; i++)
            {
                balls.Add(new BaseData<int>(drawBalls[i], "Ball" + (i + 1)));
            }

            return new Draw(balls, model);
        }

        public List<IData> SumIndexes(int factor, int sum,int baseSum)
        {
            int sumFactor = sum / baseSum;
            int sumIndex = sumFactor * baseSum;
            return new List<IData>
            {
                new FeatureData<long>(FeatureNames.SumIndex, sumIndex),
                new FeatureData<long>(FeatureNames.SumDiff, Math.Abs(sum - sumIndex)),
                new FeatureData<long>(FeatureNames.SumDiffFactor, sumFactor),
               
            };
        }


        private string GetCount(string primePattern, ICombination<int> model)
        {
            if (model is IMulitCombination<int>)
            {
                return GetMultiCount(primePattern, model as IMulitCombination<int>);
            }
            return primePattern.Count(x => x == 'Y').ToString();
        }

        private string GetMultiCount(string primePattern, IMulitCombination<int> mulitCombination)
        {
            string[] splits = primePattern.Split(':');
            string mainPattern = string.Join(":", splits.Take(mulitCombination.Main.ChosenElements));
            string subPattern = string.Join(":", splits.Skip(mulitCombination.Main.ChosenElements));

            return GetCount(mainPattern, mulitCombination.Main) + ":" + GetCount(subPattern, mulitCombination.Sub);
        }

        private string GetFiboPattern(int[] actual)
        {
            List<string> pattern = new List<string>();
            foreach (var number in actual)
            {
                if (IsFibonacci(number))
                {
                    pattern.Add("Y");
                }
                else
                {
                    pattern.Add("N");
                }
            }
            return string.Join(":", pattern);
        }
        private string GetPrimePattern(int[] actual)
        {
            List<string> pattern = new List<string>();
            foreach (var number in actual)
            {
                if (IsPrime(number))
                {
                    pattern.Add("Y");
                }
                else
                {
                    pattern.Add("N");
                }
            }
            return string.Join(":", pattern);
        }
        bool IsPrime(int number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (int i = 3; i <= boundary; i += 2)
                if (number % i == 0)
                    return false;

            return true;
        }
        bool IsPerfectSquare(int x)
        {
            int s = (int)Math.Sqrt(x);
            return (s * s == x);
        }
        bool IsFibonacci(int n)
        {
            // n is Fibonacci if one of 
            // 5*n*n + 4 or 5*n*n - 4 or  
            // both are a perfect square 
            return IsPerfectSquare(5 * n * n + 4) ||
                   IsPerfectSquare(5 * n * n - 4);
        }

       


        private IData CreateDraw(List<int> drawBalls, ICombination<int> configModel)
        {
            List<IData> balls = new List<IData>
            {
                new BaseData<DateTime>(DateTime.Today, "Date")

            };
            for (int i = 0; i < drawBalls.Count; i++)
            {
                balls.Add(new BaseData<int>(drawBalls[i], "Ball" + (i + 1)));
            }

            return new Draw(balls, configModel);
        }
        public List<IData> ToFibIndexs(BaseData<int> indexData)
        {
            long index = indexData.Data;
            return FibIndexs(index);




        }

        public bool IsVortex(int absDiff, int vortexValue)
        {
            return VortexNumber(absDiff) == vortexValue;
        }

        //private Func<long, long> Fibo = n =>
        //{
        //    int a = 1, b = 2;
        //    for (; b < n; a = b - a) b += a;
        //    return n - a > b - n ? b : a;
        //};
        public List<IData> ToBase9(IEnumerable<IData> balls)
        {
            return balls.Select(Base9Data).ToList();
        }
        public IData Base9Data(IData data)
        {
            BaseData<int> intBall = (BaseData<int>)data;
            int num = intBall.Data;
            int quot;
            string rem = "";
            while (num >= 1)
            {
                quot = num / 9;
                rem += (num % 9).ToString();
                num = quot;
            }
            string bin = "";
            for (int i = rem.Length - 1; i >= 0; i--)
            {
                bin = bin + rem[i];
            }
            return new BaseData<string>(bin, intBall.Field);
        }

        
    }
}