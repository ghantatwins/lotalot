using System.Globalization;

namespace LottoData.Lib.Interfaces.DataSets
{
    public interface IDataFormat
    {
        char Separator { get; }
        NumberFormatInfo NumberFormatInfo { get; }
        DateTimeFormatInfo DateTimeFormatInfo { get; }
        bool VariableNamesAvailable { get; }
    }
}