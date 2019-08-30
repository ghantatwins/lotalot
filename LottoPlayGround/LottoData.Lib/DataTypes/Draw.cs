using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lotto.Combinatorics;
using LottoData.Lib.Interfaces.DataTypes;

namespace LottoData.Lib.DataTypes
{
    public class Draw:IDraw, IEquatable<Draw>
    {
        private readonly ICombination<int> _model;
        private readonly List<IData> _dataForm=new List<IData>();

       

        public Draw(IList<IData> rawData, ICombination<int> model)
        {
            _model = model;
            BallsArray = rawData.OfType<BaseData<int>>().Select(y=>y.Data).ToArray();
            DrawDate = DateData(rawData).Data;
           _dataForm.AddRange(rawData);
        }

        private static BaseData<DateTime> DateData(IList<IData> rawData)
        {
            return rawData.OfType<BaseData<DateTime>>().SingleOrDefault();
        }

        public DateTime DrawDate { get; }

        public int GetBallByIndex(int index)
        {
            return BallsArray[index];
        }

        public int[] BallsArray { get; }
        public long BallsSum
        {
            get { return BallsArray.Sum(); }
           
        }
    
    public long PhysicalPosition
        {
        get { return ModelIndex + 1; }
          
        }

        public long ModelIndex
        {
        get { return (long)_model.GetIndexOf(BallsArray, Comparer<int>.Default); ; }
          
        }

        public decimal Proportion
        {
        get { return (decimal)PhysicalPosition / _model.TotalCombinations; }
            
        }

        public IDraw Add(IDraw draw)
        {
            int[] newDraw= new int[BallsArray.Length];
            for (int i = 0; i < BallsArray.Length; i++)
            {
                newDraw[i] = BallsArray[i] + draw.BallsArray[i];
            }
            return (IDraw) CreateDraw(newDraw, DrawDate);
        }
        private IData CreateDraw(int[] drawBalls, DateTime drawDate)
        {
            List<IData> balls = new List<IData>
            {
                new BaseData<DateTime>(drawDate,"Date")
            };
            for (int i = 0; i < drawBalls.Length; i++)
            {
                balls.Add(new BaseData<int>(drawBalls[i], "Ball" + (i + 1)));
            }

            return new Draw(balls, _model);
        }
        public IDraw Subtract(IDraw draw)
        {
            int[] newDraw = new int[BallsArray.Length];
            for (int i = 0; i < BallsArray.Length; i++)
            {
                newDraw[i] = BallsArray[i] - draw.BallsArray[i];
            }
            return (IDraw)CreateDraw(newDraw, DrawDate);
        }

        public IDraw Multiply(IDraw draw)
        {
            int[] newDraw = new int[BallsArray.Length];
            for (int i = 0; i < BallsArray.Length; i++)
            {
                newDraw[i] = BallsArray[i] * draw.BallsArray[i];
            }
            return (IDraw)CreateDraw(newDraw, DrawDate);
        }

        public IDraw Divide(IDraw draw)
        {
            int[] newDraw = new int[BallsArray.Length];
            for (int i = 0; i < BallsArray.Length; i++)
            {
                newDraw[i] = BallsArray[i] / draw.BallsArray[i];
            }
            return (IDraw)CreateDraw(newDraw, DrawDate);
        }

        public IDraw Add(double value)
        {
            int[] newDraw = new int[BallsArray.Length];
            for (int i = 0; i < BallsArray.Length; i++)
            {
                newDraw[i] = BallsArray[i] + (int)value;
            }
            return (IDraw)CreateDraw(newDraw, DrawDate);
        }

        public IDraw Subtract(double value)
        {
            int[] newDraw = new int[BallsArray.Length];
            for (int i = 0; i < BallsArray.Length; i++)
            {
                newDraw[i] = BallsArray[i] - (int)value;
            }
            return (IDraw)CreateDraw(newDraw, DrawDate);
        }

        public IDraw Multiply(double value)
        {
            int[] newDraw = new int[BallsArray.Length];
            for (int i = 0; i < BallsArray.Length; i++)
            {
                newDraw[i] = (int)(BallsArray[i] * value);
            }
            return (IDraw)CreateDraw(newDraw, DrawDate);
        }

        public IDraw Divide(double value)
        {
            int[] newDraw = new int[BallsArray.Length];
            for (int i = 0; i < BallsArray.Length; i++)
            {
                newDraw[i] = (int)(BallsArray[i] / value);
            }
            return (IDraw)CreateDraw(newDraw, DrawDate);
        }

        public int SumDigitsSum
        {
            get
            {
                return VortexNumber(BallsSum);
            }
        }

        private int VortexNumber(long number)
        {
            
            while (number > 9)
            {
                number = GetIntArray(number).Sum();
            
            }

            return (int)number;
        }

        
        long[] GetIntArray(long num)
        {
            List<long> listOfInts = new List<long>();
            while (num > 0)
            {
                listOfInts.Add(num % 10);
                num = num / 10;
            }
            listOfInts.Reverse();
            return listOfInts.ToArray();
        }
        public int IndexDigitsSum
        {
            get
            {
                return VortexNumber(ModelIndex);
            }
        }
        public bool Equals(IData other)
        {
            return Equals(other as Draw);
        }

        public bool Equals(Draw other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return BallsArray.SequenceEqual(other.BallsArray); 
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Draw) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode =  (BallsArray != null ? BallsArray.GetHashCode() : 0);
                return hashCode;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static bool operator ==(Draw left, Draw right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Draw left, Draw right)
        {
            return !Equals(left, right);
        }

        public IEnumerator<IData> GetEnumerator()
        {
            return _dataForm.GetEnumerator();
        }

        public string ToVerboseString()
        {
            return string.Format(
                "DrawDate:{0},Balls:{1},BallSum:{2},PhysicalPosition:{3},Index:{4},Proportion:{5},SumDigitsSum:{6},IndexDigitsSum:{7}",
                DrawDate, string.Join(".", BallsArray), BallsSum, PhysicalPosition, ModelIndex, Proportion,
                SumDigitsSum, IndexDigitsSum);
        }
        public override string ToString()
        {
            return string.Format(
                "Balls:{0}",
                string.Join(".", BallsArray));
        }
    }
}
