using System.Collections.Generic;
using LottoData.Lib.Factories;
using LottoData.Lib.Interfaces;
using LottoData.Lib.Interfaces.DataTypes;
using LottoData.Lib.Interfaces.Factories;
using LottoData.Lib.Interfaces.Providers.Lottery;
using LottoData.Lib.Providers.Lottery;

namespace LottoData.Lib
{
    public class FeaturesMultiLottoManager : IMultiLottoManager
    {
        private readonly IMultiLottoManager _manager;
        private readonly LotteryProvider _provider;
        


        public FeaturesMultiLottoManager(IMultiLottoManager manager)
        {
            _manager = manager;
            DataFactory = new LottoFeaturesDataFactory(manager.DataFactory, manager.Spec.Features);
            _provider = new LotteryProvider(DataFactory);
        }
        private IList<IData> _data;
        public IList<IData> Data
        {
            get
            {
                if(_data==null) _data= _provider.ImportData(Path);
                return _data;
            }
        }

        public ILotteryDataFactory DataFactory { get; }

        public IMultiLotterySpec Spec => _manager.Spec;

        public string Path => _manager.Path;
        public void Export(string path)
        {
            _provider.ExportData(Data,path);
        }
    }
}