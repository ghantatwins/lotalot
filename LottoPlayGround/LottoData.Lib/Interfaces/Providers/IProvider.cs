using System.Collections.Generic;
using LottoData.Lib.Interfaces.DataSets;
using LottoData.Lib.Interfaces.DataTypes;

namespace LottoData.Lib.Interfaces.Providers
{
    public interface IProvider
    {
        IDataSet ImportDataSet(string path, IDataFormat properties);
        IList<IData> ImportData(string path, IDataFormat properties);
        IList<IData> ImportData(IDataSet instance);
        IList<IData> ImportData(string path, IDataFormat properties, ITableFileParser csvFileParser);
        void ExportData(IDataSet instance, string path);
        void ExportData(IList<IData> instance,  string path);
        //void ExportToExcel(IData instance, string path);
    }
}