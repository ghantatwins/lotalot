using System.Collections.Generic;
using LottoData.Lib.Factories;
using LottoData.Lib.Interfaces;
using LottoData.Lib.Interfaces.DataTypes;
using LottoData.Lib.Interfaces.Factories;
using LottoData.Lib.Interfaces.Providers.Lottery;
using LottoData.Lib.Providers.Lottery;

namespace LottoData.Lib
{
    public class MultiLottoManager : IMultiLottoManager
    {
        private readonly LotteryProvider _provider;
        public string Path { get; }
        public void Export(string path)
        {
            _provider.ExportData(Data, path);
        }


        public MultiLottoManager(string path) : this(new EuroJackpotSpec(), path)
        {

        }

        private MultiLottoManager(IMultiLotterySpec spec, string path) : this(path, spec,new MultiLotteryDataFactory(new Lottery(spec)))
        {

        }


        public MultiLottoManager(string path, IMultiLotterySpec spec, ILotteryDataFactory dataFactory)
        {
            Path = path;
            Spec = spec;
            DataFactory = dataFactory;
            _provider = new LotteryProvider(DataFactory);
        }

        private IList<IData> _data;
        public IList<IData> Data
        {
            get
            {
                if (_data == null) _data = _provider.ImportData(Path);
                return _data;
            }
        }


        public ILotteryDataFactory DataFactory { get; }

        public IMultiLotterySpec Spec { get; }

        
    }
}