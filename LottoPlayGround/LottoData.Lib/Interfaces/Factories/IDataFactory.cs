using System.Collections.Generic;
using LottoData.Lib.Interfaces.DataSets;
using LottoData.Lib.Interfaces.DataTypes;

namespace LottoData.Lib.Interfaces.Factories
{
    public interface IDataFactory
    {
       
        IEnumerable<IData> CreateDataRows(IDataSet dataSet);

        IList<IData> GetColumnData(IData data);
        IEnumerable<string> GetVariables(IList<IData> instance);
        string GetData(IData data);
    }
}
