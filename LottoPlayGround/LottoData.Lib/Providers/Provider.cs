using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using LottoData.Lib.DataSets;
using LottoData.Lib.Interfaces.DataSets;
using LottoData.Lib.Interfaces.DataTypes;
using LottoData.Lib.Interfaces.Factories;
using LottoData.Lib.Interfaces.Providers;

namespace LottoData.Lib.Providers
{
    public class Provider : IProvider
    {
        private readonly IDataFactory _factory;

        public Provider(IDataFactory factory)
        {
            _factory = factory;
        }
        protected virtual void OnProgressChanged(double d)
        {
            var handler = ProgressChanged;
            if (handler != null)
                handler(this, new ProgressChangedEventArgs((int)(100 * d), null));
        }
        public virtual IList<IData> ImportData(string path, IDataFormat properties, ITableFileParser csvFileParser)
        {
            IDataSet database = ImportDataSet(path, properties);
            return ImportData(database);
        }
        public virtual IList<IData> ImportData(IDataSet dataSet)
        {
            List<IData> newDataList = new List<IData>();
            newDataList.AddRange(_factory.CreateDataRows(dataSet));
            return newDataList;
        }
        public virtual IDataSet ImportDataSet(string path, IDataFormat properties)
        {
            var csvFileParser = GetParser(path, properties);
            return new DataSet(csvFileParser.VariableNames.ToList(), csvFileParser.Values); ;
        }
        public virtual IList<IData> ImportData(string path, IDataFormat properties)
        {
            return ImportData(path, properties, GetParser(path, properties));
        }

        private TableFileParser GetParser(string path, IDataFormat properties)
        {
            TableFileParser csvFileParser = new TableFileParser();
            var fileSize = new FileInfo(path).Length;
            csvFileParser.ProgressChanged += (sender, e) => { OnProgressChanged(e / (double)fileSize); };
            csvFileParser.Parse(path, properties.NumberFormatInfo, properties.DateTimeFormatInfo, properties.Separator,
                properties.VariableNamesAvailable
                    ? csvFileParser.AreColumnNamesInFirstLine(path)
                    : properties.VariableNamesAvailable);
            return csvFileParser;
        }

        public event ProgressChangedEventHandler ProgressChanged;

        protected void Throw(Exception exception)
        {
            if (exception != null) throw exception;
        }

        protected void ReleaseObject(object obj, ref Exception exception)
        {

            try
            {

                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                AssignNull(out obj);
            }
            catch (Exception ex)
            {
                AssignNull(out obj);
                exception = new Exception("Unable to release the Object ", ex);

            }
            finally
            {
                GC.Collect();
            }

        }

        private void AssignNull(out object refobject)
        {
            refobject = null;
        }

        public void ExportData(IDataSet instance, string path)
        {
            var strBuilder = new StringBuilder();
            var colSep = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            foreach (var variable in instance.VariableNames)
            {
                strBuilder.Append(variable.Replace(colSep, string.Empty) + colSep);
            }
            strBuilder.Remove(strBuilder.Length - colSep.Length, colSep.Length);
            strBuilder.AppendLine();

            var dataset = instance;

            for (var i = 0; i < dataset.Rows; i++)
            {
                for (var j = 0; j < dataset.Columns; j++)
                {
                    if (j > 0) strBuilder.Append(colSep);
                    strBuilder.Append(dataset.GetValue(i, j));
                }
                strBuilder.AppendLine();
            }
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                var encoding = Encoding.GetEncoding(Encoding.Default.CodePage,
                    new EncoderReplacementFallback("*"),
                    new DecoderReplacementFallback("*"));
                using (var writer = new StreamWriter(fileStream, encoding))
                {
                    writer.Write(strBuilder);
                }
            }
        }

        public void ExportData(IList<IData> instance, string path)
        {
            var strBuilder = new StringBuilder();
            var colSep = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            IEnumerable<string> variables = _factory.GetVariables(instance);
            foreach (var variable in variables)
            {
                strBuilder.Append(variable.Replace(colSep, string.Empty) + colSep);
            }
            strBuilder.Remove(strBuilder.Length - colSep.Length, colSep.Length);
            strBuilder.AppendLine();



            for (var i = 0; i < instance.Count; i++)
            {
                IList<IData> columnData = _factory.GetColumnData(instance[i]);
                for (var j = 0; j < columnData.Count; j++)
                {
                    if (j > 0) strBuilder.Append(colSep);
                    strBuilder.Append(_factory.GetData(columnData[j]));
                }
                strBuilder.AppendLine();
            }
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                var encoding = Encoding.GetEncoding(Encoding.Default.CodePage,
                    new EncoderReplacementFallback("*"),
                    new DecoderReplacementFallback("*"));
                using (var writer = new StreamWriter(fileStream, encoding))
                {
                    writer.Write(strBuilder);
                }
            }
        }
    }
}