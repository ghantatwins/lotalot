using System.Globalization;
using LottoData.Lib.Interfaces.DataSets;

namespace LottoData.Lib.DataSets
{
    public class CsvSeFormat:IDataFormat
    {
        public char Separator
        {
            get { return ';'; }
        }
        public NumberFormatInfo NumberFormatInfo
        {
            get { return CultureInfo.CurrentCulture.NumberFormat; }
        }
        public DateTimeFormatInfo DateTimeFormatInfo
        {
            get { return CultureInfo.CurrentCulture.DateTimeFormat; }
        }
        public bool VariableNamesAvailable { get; set; }
    }
}