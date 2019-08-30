using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Lotto.Combinatorics;
using LottoData.Lib.DataTypes;
using LottoData.Lib.Interfaces.DataSets;
using LottoData.Lib.Interfaces.DataTypes;
using LottoData.Lib.Interfaces.Factories;
using LottoData.Lib.Interfaces.Providers.Lottery;

namespace LottoData.Lib.Factories
{
    public class MultiLotteryDataFactory : ILotteryDataFactory
    {


        public ILottoConfig Config { get; }



        public MultiLotteryDataFactory(ILottoConfig config)
        {

            Config = config;


        }

        public IEnumerable<IData> CreateDataRows(IDataSet dataSet)
        {
            List<IData> rows = new List<IData>();
            var headers = dataSet.VariableNames.ToList();


            List<DateTime> drawDates = new List<DateTime>();

            Dictionary<string, List<int>> balls = new Dictionary<string, List<int>>();
            foreach (var header in headers)
            {
                if (dataSet.VariableHasType<DateTime>(header))
                {
                    drawDates.AddRange(dataSet.GetDateTimeValues(header));
                }
                if (dataSet.VariableHasType<double>(header))
                {
                    balls.Add(header, dataSet.GetValues<double>(header).Select(x => (int)x).ToList());
                }

            }

            for (int i = 0; i < drawDates.Count; i++)
            {
                List<int> drawBalls = new List<int>();
                var ballKeys = balls.Keys.ToList();
                var ballKeyCount = ballKeys.Count;


                for (var key = 0; key < ballKeyCount; key++)
                {
                    drawBalls.Add(balls[ballKeys[key]][i]);
                }

                if (Config.Model is IMulitCombination<int>)
                {
                    IMulitCombination<int> model = (IMulitCombination<int>)Config.Model;
                    List<int> mainBalls = new List<int>(drawBalls.Take(model.Main.ChosenElements));
                    mainBalls.Sort();
                    List<int> starBalls = new List<int>(drawBalls.Skip(model.Main.ChosenElements));
                    starBalls.Sort();
                    drawBalls = new List<int>();
                    drawBalls.AddRange(mainBalls);
                    drawBalls.AddRange(starBalls);
                }
                else
                {
                    drawBalls.Sort();
                }

               rows.Add(CreateDraw(drawBalls, Config.Model, drawDates[i]));



            }

            return rows;
        }

        private IData CreateDraw(List<int> drawBalls, ICombination<int> configModel, DateTime drawDate)
        {
           List<IData> balls = new List<IData>
           {
               new BaseData<DateTime>(drawDate,"Date")

           };
            for (int i = 0; i < drawBalls.Count; i++)
            {
                balls.Add(new BaseData<int>(drawBalls[i], "Ball" + (i + 1)));
            }

            return new Draw(balls, configModel);
        }

        public IList<IData> GetColumnData(IData data)
        {
            IDraw draw = (IDraw)data;
            return draw.ToList();
        }

        public IEnumerable<string> GetVariables(IList<IData> instance)
        {
            IData rowData = instance[0];
            IList<IData> columns = GetColumnData(rowData);
            return columns.Select(x => x.ToString());
        }



        public string GetData(IData data)
        {
            if (data is FeatureData<long>)
            {
                return ((FeatureData<long>)data).Value.ToString();
            }
            if (data is FeatureData<string>)
            {
                return ((FeatureData<string>)data).Value;
            }
            if (data is BaseData<IData>)
            {
                return GetData(AsBaseData<IData>(data).Data);
            }
            if (data is BaseData<int>)
            {
                return AsBaseData<int>(data).Data.ToString();
            }
            if (data is BaseData<double>)
            {
                return AsBaseData<double>(data).Data.ToString(CultureInfo.InvariantCulture);
            }
            if (data is BaseData<DateTime>)
            {
                return AsBaseData<DateTime>(data).Data.ToShortDateString();
            }
            return AsBaseData<string>(data).Data;
        }

        private static BaseData<T> AsBaseData<T>(IData data)
        {
            return (data as BaseData<T>);
        }



    }
}