using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace LottoData.Lib.Interfaces.DataSets
{
    public interface ITableFileParser
    {
        /// <summary>
        /// Total Number of Rows
        /// </summary>
        int Rows { get; set; }
        /// <summary>
        /// Total Number of Columns
        /// </summary>
        int Columns { get; set; }
        /// <summary>
        /// Total Values in List of collections
        /// </summary>
        List<IList> Values { get; }
        /// <summary>
        /// Variable Names
        /// </summary>
        IEnumerable<string> VariableNames { get; }

        /// <summary>
        /// Checks whether column names in first line from a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        bool AreColumnNamesInFirstLine(string fileName);
        /// <summary>
        /// Checks if there a column names in first line from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        bool AreColumnNamesInFirstLine(Stream stream);
        /// <summary>
        /// Checks for column names in first line
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="numberFormat"></param>
        /// <param name="dateTimeFormatInfo"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        bool AreColumnNamesInFirstLine(string fileName, NumberFormatInfo numberFormat,
            DateTimeFormatInfo dateTimeFormatInfo, char separator);
        /// <summary>
        /// Checks for column names in first line
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="numberFormat"></param>
        /// <param name="dateTimeFormatInfo"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        bool AreColumnNamesInFirstLine(Stream stream, NumberFormatInfo numberFormat,
            DateTimeFormatInfo dateTimeFormatInfo, char separator);

        /// <summary>
        ///     Parses a file and determines the format first
        /// </summary>
        /// <param name="fileName">file which is parsed</param>
        /// <param name="columnNamesInFirstLine"></param>
        /// <param name="lineLimit"></param>
        void Parse(string fileName, bool columnNamesInFirstLine, int lineLimit = -1);

        /// <summary>
        ///     Parses a file with the given formats
        /// </summary>
        /// <param name="fileName">file which is parsed</param>
        /// <param name="numberFormat">Format of numbers</param>
        /// <param name="dateTimeFormatInfo">Format of datetime</param>
        /// <param name="separator">defines the separator</param>
        /// <param name="columnNamesInFirstLine"></param>
        /// <param name="lineLimit"></param>
        void Parse(string fileName, NumberFormatInfo numberFormat, DateTimeFormatInfo dateTimeFormatInfo,
            char separator, bool columnNamesInFirstLine, int lineLimit = -1);

        /// <summary>
        ///     Takes a Stream and parses it with default format. NumberFormatInfo.InvariantInfo, DateTimeFormatInfo.InvariantInfo
        ///     and separator = ','
        /// </summary>
        /// <param name="stream">stream which is parsed</param>
        /// <param name="columnNamesInFirstLine"></param>
        /// <param name="lineLimit"></param>
        void Parse(Stream stream, bool columnNamesInFirstLine, int lineLimit = -1);

        /// <summary>
        ///     Parses a stream with the given formats.
        /// </summary>
        /// <param name="stream">Stream which is parsed</param>
        /// <param name="numberFormat">Format of numbers</param>
        /// <param name="dateTimeFormatInfo">Format of datetime</param>
        /// <param name="separator">defines the separator</param>
        /// <param name="columnNamesInFirstLine"></param>
        /// <param name="lineLimit"></param>
        void Parse(Stream stream, NumberFormatInfo numberFormat, DateTimeFormatInfo dateTimeFormatInfo,
            char separator, bool columnNamesInFirstLine, int lineLimit = -1);

       
    }
}