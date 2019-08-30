using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using LottoData.Lib.Interfaces.DataSets;

namespace LottoData.Lib.DataSets
{
    public class TableFileParser : Progress<long>, ITableFileParser
    {
        // reports the number of bytes read
        private const int BufferSize = 65536;
        // char used to symbolize whitespaces (no missing values can be handled with whitespaces)
        private const char Whitespacechar = (char)0;
        private static readonly char[] PossibleSeparators = { ',', ';', '\t', Whitespacechar };


        private Encoding _encoding = Encoding.Default;

        private int _estimatedNumberOfLines = 200;
        // initial capacity for columns, will be set automatically when data is read from a file


        private Tokenizer _tokenizer;

        private List<string> _variableNames;

        public TableFileParser()
        {
            _variableNames = new List<string>();
        }

        public Encoding Encoding
        {
            get { return _encoding; }
            set
            {
                if (value == null) throw new Exception("Encoding");
                _encoding = value;
            }
        }

        public int Rows { get; set; }

        public int Columns { get; set; }

        public List<IList> Values { get; private set; }

        public IEnumerable<string> VariableNames
        {
            get
            {
                if (_variableNames.Count > 0) return _variableNames;
                var names = new string[Columns];
                for (var i = 0; i < names.Length; i++)
                {
                    names[i] = "X" + i.ToString("000");
                }
                return names;
            }
        }

        public bool AreColumnNamesInFirstLine(string fileName)
        {
            NumberFormatInfo numberFormat;
            DateTimeFormatInfo dateTimeFormatInfo;
            char separator;
            DetermineFileFormat(fileName, out numberFormat, out dateTimeFormatInfo, out separator);
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return AreColumnNamesInFirstLine(stream, numberFormat, dateTimeFormatInfo, separator);
            }
        }

        public bool AreColumnNamesInFirstLine(Stream stream)
        {
            var numberFormat = NumberFormatInfo.InvariantInfo;
            var dateTimeFormatInfo = DateTimeFormatInfo.InvariantInfo;
            var separator = ',';
            return AreColumnNamesInFirstLine(stream, numberFormat, dateTimeFormatInfo, separator);
        }

        public bool AreColumnNamesInFirstLine(string fileName, NumberFormatInfo numberFormat,
            DateTimeFormatInfo dateTimeFormatInfo, char separator)
        {
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return AreColumnNamesInFirstLine(stream, numberFormat, dateTimeFormatInfo, separator);
            }
        }

        public bool AreColumnNamesInFirstLine(Stream stream, NumberFormatInfo numberFormat,
            DateTimeFormatInfo dateTimeFormatInfo, char separator)
        {
            using (var reader = new StreamReader(stream, Encoding))
            {
                _tokenizer = new Tokenizer(reader, numberFormat, dateTimeFormatInfo, separator);
                return (_tokenizer.PeekType() != TokenTypeEnum.Double);
            }
        }

        /// <summary>
        ///     Parses a file and determines the format first
        /// </summary>
        /// <param name="fileName">file which is parsed</param>
        /// <param name="columnNamesInFirstLine"></param>
        /// <param name="lineLimit"></param>
        public void Parse(string fileName, bool columnNamesInFirstLine, int lineLimit = -1)
        {
            NumberFormatInfo numberFormat;
            DateTimeFormatInfo dateTimeFormatInfo;
            char separator;
            DetermineFileFormat(fileName, out numberFormat, out dateTimeFormatInfo, out separator);
            EstimateNumberOfLines(fileName);
            Parse(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), numberFormat,
                dateTimeFormatInfo, separator, columnNamesInFirstLine, lineLimit);
        }

        /// <summary>
        ///     Parses a file with the given formats
        /// </summary>
        /// <param name="fileName">file which is parsed</param>
        /// <param name="numberFormat">Format of numbers</param>
        /// <param name="dateTimeFormatInfo">Format of datetime</param>
        /// <param name="separator">defines the separator</param>
        /// <param name="columnNamesInFirstLine"></param>
        /// <param name="lineLimit"></param>
        public void Parse(string fileName, NumberFormatInfo numberFormat, DateTimeFormatInfo dateTimeFormatInfo,
            char separator, bool columnNamesInFirstLine, int lineLimit = -1)
        {
            EstimateNumberOfLines(fileName);
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                Parse(stream, numberFormat, dateTimeFormatInfo, separator, columnNamesInFirstLine, lineLimit);
            }
        }

        // determines the number of newline characters in the first 64KB to guess the number of rows for a file
        private void EstimateNumberOfLines(string fileName)
        {
            var len = new FileInfo(fileName).Length;
            var buf = new char[1024 * 1024];
            using (var reader = new StreamReader(fileName, Encoding))
            {
                reader.ReadBlock(buf, 0, buf.Length);
            }
            var numNewLine = 0;
            int charsInCurrentLine = 0, charsInFirstLine = 0;
            // the first line (names) and the last line (incomplete) are not representative
            foreach (var ch in buf)
            {
                charsInCurrentLine++;
                if (ch == '\n')
                {
                    if (numNewLine == 0)
                        charsInFirstLine = charsInCurrentLine; // store the number of chars in the first line
                    charsInCurrentLine = 0;
                    numNewLine++;
                }
            }
            if (numNewLine <= 1)
            {
                // fail -> keep the default setting
            }
            else
            {
                var charsPerLineFactor = (buf.Length - charsInFirstLine - charsInCurrentLine) / ((double)numNewLine - 1);
                var estimatedLines = len / charsPerLineFactor;
                _estimatedNumberOfLines = (int)Math.Round(estimatedLines * 1.1);
                // pessimistic allocation of 110% to make sure that the list is very likely large enough
            }
        }

        /// <summary>
        ///     Takes a Stream and parses it with default format. NumberFormatInfo.InvariantInfo, DateTimeFormatInfo.InvariantInfo
        ///     and separator = ','
        /// </summary>
        /// <param name="stream">stream which is parsed</param>
        /// <param name="columnNamesInFirstLine"></param>
        /// <param name="lineLimit"></param>
        public void Parse(Stream stream, bool columnNamesInFirstLine, int lineLimit = -1)
        {
            var numberFormat = NumberFormatInfo.InvariantInfo;
            var dateTimeFormatInfo = DateTimeFormatInfo.InvariantInfo;
            var separator = ',';
            Parse(stream, numberFormat, dateTimeFormatInfo, separator, columnNamesInFirstLine, lineLimit);
        }

        /// <summary>
        ///     Parses a stream with the given formats.
        /// </summary>
        /// <param name="stream">Stream which is parsed</param>
        /// <param name="numberFormat">Format of numbers</param>
        /// <param name="dateTimeFormatInfo">Format of datetime</param>
        /// <param name="separator">defines the separator</param>
        /// <param name="columnNamesInFirstLine"></param>
        /// <param name="lineLimit"></param>
        public void Parse(Stream stream, NumberFormatInfo numberFormat, DateTimeFormatInfo dateTimeFormatInfo,
            char separator, bool columnNamesInFirstLine, int lineLimit = -1)
        {
            using (var reader = new StreamReader(stream, Encoding))
            {
                _tokenizer = new Tokenizer(reader, numberFormat, dateTimeFormatInfo, separator);
                Values = new List<IList>();
                if (lineLimit > 0) _estimatedNumberOfLines = lineLimit;

                if (columnNamesInFirstLine)
                {
                    ParseVariableNames();
                    if (!_tokenizer.HasNext())
                        Error(
                            "Couldn't parse data values. Probably because of incorrect number format (the parser expects english number format with a '.' as decimal separator).",
                            "", _tokenizer.CurrentLineNumber);
                }


                // read values... start in first row 
                var nLinesParsed = 0;
                var colIdx = 0;
                var numValuesInFirstRow = columnNamesInFirstLine ? _variableNames.Count : -1;
                // number of variables or inizialize based on first row of values (-1)
                while (_tokenizer.HasNext() && (lineLimit < 0 || nLinesParsed < lineLimit))
                {
                    if (_tokenizer.PeekType() == TokenTypeEnum.NewLine)
                    {
                        _tokenizer.Skip();

                        // all rows have to have the same number of values
                        // the first row defines how many samples are needed
                        if (numValuesInFirstRow < 0)
                            numValuesInFirstRow = Values.Count; // set to number of colums in the first row
                        else if (colIdx > 0 && numValuesInFirstRow != colIdx)
                        {
                            // read at least one value in the row (support for skipping empty lines)
                            Error(
                                "The first row of the dataset has " + numValuesInFirstRow + " columns." +
                                Environment.NewLine +
                                "Line " + _tokenizer.CurrentLineNumber + " has " + colIdx + " columns.", "",
                                _tokenizer.CurrentLineNumber);
                        }
                        OnReport(_tokenizer.BytesRead);

                        nLinesParsed++;
                        colIdx = 0;
                    }
                    else
                    {
                        // read one value
                        TokenTypeEnum type;
                        string strVal;
                        double dblVal;
                        DateTime dateTimeVal;
                        _tokenizer.Next(out type, out strVal, out dblVal, out dateTimeVal);

                        // initialize columns on the first row (fixing data types as presented in the first row...)
                        if (nLinesParsed == 0)
                        {
                            Values.Add(CreateList(type, _estimatedNumberOfLines));
                        }
                        else if (colIdx == Values.Count)
                        {
                            Error(
                                "The first row of the dataset has " + numValuesInFirstRow + " columns." +
                                Environment.NewLine +
                                "Line " + _tokenizer.CurrentLineNumber + " has more columns.", "",
                                _tokenizer.CurrentLineNumber);
                        }
                        if (!IsColumnTypeCompatible(Values[colIdx], type))
                        {
                            Values[colIdx] = ConvertToStringColumn(Values[colIdx]);
                        }
                        // add the value to the column
                        AddValue(type, Values[colIdx++], strVal, dblVal, dateTimeVal);
                    }
                }

                if (!Values.Any() || Values.First().Count == 0)
                    Error("Couldn't parse data values. Probably because of incorrect number format " +
                          "(the parser expects english number format with a '.' as decimal separator).", "",
                        _tokenizer.CurrentLineNumber);
            }

            Rows = Values.First().Count;
            Columns = Values.Count;

            // after everything has been parsed make sure the lists are as compact as possible
            foreach (var l in Values)
            {
                var dblList = l as List<double>;
                var byteList = l as List<byte>;
                var dateList = l as List<DateTime>;
                var stringList = l as List<string>;
                var objList = l as List<object>;
                if (dblList != null) dblList.TrimExcess();
                if (byteList != null) byteList.TrimExcess();
                if (dateList != null) dateList.TrimExcess();
                if (stringList != null) stringList.TrimExcess();
                if (objList != null) objList.TrimExcess();
            }

            // for large files we created a lot of memory pressure, cannot hurt to run GC.Collect here (TableFileParser is called seldomly on user interaction)
            GC.Collect(2, GCCollectionMode.Forced);
        }

        public static void DetermineFileFormat(string path, out NumberFormatInfo numberFormat,
            out DateTimeFormatInfo dateTimeFormatInfo, out char separator)
        {
            DetermineFileFormat(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite),
                out numberFormat, out dateTimeFormatInfo, out separator);
        }

        public static void DetermineFileFormat(Stream stream, out NumberFormatInfo numberFormat,
            out DateTimeFormatInfo dateTimeFormatInfo, out char separator)
        {
            using (var reader = new StreamReader(stream))
            {
                // skip first line
                reader.ReadLine();
                // read a block
                var buffer = new char[BufferSize];
                var charsRead = reader.ReadBlock(buffer, 0, BufferSize);
                // count frequency of special characters
                var charCounts = buffer.Take(charsRead)
                    .GroupBy(c => c)
                    .ToDictionary(g => g.Key, g => g.Count());

                // depending on the characters occuring in the block 
                // we distinghish a number of different cases based on the the following rules:
                // many points => it must be English number format, the other frequently occuring char is the separator
                // no points but many commas => this is the problematic case. Either German format (real numbers) or English format (only integer numbers) with ',' as separator
                //   => check the line in more detail:
                //            English: 0, 0, 0, 0
                //            German:  0,0 0,0 0,0 ...
                //            => if commas are followed by space => English format
                // no points no commas => English format (only integer numbers) use the other frequently occuring char as separator
                // in all cases only treat ' ' as separator if no other separator is possible (spaces can also occur additionally to separators)
                if (OccurrencesOf(charCounts, '.') > 10)
                {
                    numberFormat = NumberFormatInfo.InvariantInfo;
                    dateTimeFormatInfo = DateTimeFormatInfo.InvariantInfo;
                    separator = PossibleSeparators
                        .Where(c => OccurrencesOf(charCounts, c) > 10)
                        .OrderBy(c => -OccurrencesOf(charCounts, c))
                        .DefaultIfEmpty(' ')
                        .First();
                }
                else if (OccurrencesOf(charCounts, ',') > 10)
                {
                    // no points and many commas
                    // count the number of tokens (chains of only digits and commas) that contain multiple comma characters
                    var tokensWithMultipleCommas = 0;
                    for (var i = 0; i < charsRead; i++)
                    {
                        var nCommas = 0;
                        while (i < charsRead && (buffer[i] == ',' || char.IsDigit(buffer[i])))
                        {
                            if (buffer[i] == ',') nCommas++;
                            i++;
                        }
                        if (nCommas > 2) tokensWithMultipleCommas++;
                    }
                    if (tokensWithMultipleCommas > 1)
                    {
                        // English format (only integer values) with ',' as separator
                        numberFormat = NumberFormatInfo.InvariantInfo;
                        dateTimeFormatInfo = DateTimeFormatInfo.InvariantInfo;
                        separator = ',';
                    }
                    else
                    {
                        char[] disallowedSeparators = { ',' };
                        // n. def. contains a space so ' ' should be disallowed to, however existing unit tests would fail
                        // German format (real values)
                        numberFormat = NumberFormatInfo.GetInstance(new CultureInfo("de-DE"));
                        dateTimeFormatInfo = DateTimeFormatInfo.GetInstance(new CultureInfo("de-DE"));
                        separator = PossibleSeparators
                            .Except(disallowedSeparators)
                            .Where(c => OccurrencesOf(charCounts, c) > 10)
                            .OrderBy(c => -OccurrencesOf(charCounts, c))
                            .DefaultIfEmpty(' ')
                            .First();
                    }
                }
                else
                {
                    // no points and no commas => English format
                    numberFormat = NumberFormatInfo.InvariantInfo;
                    dateTimeFormatInfo = DateTimeFormatInfo.InvariantInfo;
                    separator = PossibleSeparators
                        .Where(c => OccurrencesOf(charCounts, c) > 10)
                        .OrderBy(c => -OccurrencesOf(charCounts, c))
                        .DefaultIfEmpty(' ')
                        .First();
                }
            }
        }

        private static int OccurrencesOf(Dictionary<char, int> charCounts, char c)
        {
            return charCounts.ContainsKey(c) ? charCounts[c] : 0;
        }

        [Serializable]
        public class DataFormatException : Exception
        {
            public DataFormatException(string message, string token, int line)
                : base(message + "\nToken: " + token + " (line: " + line + ")")
            {
                Token = token;
                Line = line;
            }

            public DataFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }

            public int Line { get; }

            public string Token { get; }
        }

        #region type-dependent dispatch

        private bool IsColumnTypeCompatible(IList list, TokenTypeEnum tokenType)
        {
            return (list is List<string>) || // all tokens can be added to a string list
                   (tokenType == TokenTypeEnum.Missing) || // empty entries are allowed in all columns
                   (tokenType == TokenTypeEnum.Double && list is List<double>) ||
                   (tokenType == TokenTypeEnum.DateTime && list is List<DateTime>);
        }

        // all columns are converted to string columns when we find an non-empty value that has incorrect type
        private IList ConvertToStringColumn(IList list)
        {
            var dblL = list as List<double>;
            if (dblL != null)
            {
                var l = new List<string>(dblL.Capacity);
                l.AddRange(dblL.Select(dbl => dbl.ToString(CultureInfo.InvariantCulture)));
                return l;
            }

            var dtL = list as List<DateTime>;
            if (dtL != null)
            {
                var l = new List<string>(dtL.Capacity);
                l.AddRange(dtL.Select(dbl => dbl.ToString(CultureInfo.InvariantCulture)));
                return l;
            }

            if (list is List<string>) return list;

            throw new InvalidProgramException($"Cannot convert column of type {list.GetType()} to string column");
        }

        private void AddValue(TokenTypeEnum type, IList list, string strVal, double dblVal, DateTime dateTimeVal)
        {
            var dblList = list as List<double>;
            if (dblList != null)
            {
                AddValue(type, dblList, dblVal);
                return;
            }

            var strList = list as List<string>;
            if (strList != null)
            {
                AddValue(type, strList, strVal);
                return;
            }
            var dtList = list as List<DateTime>;
            if (dtList != null)
            {
                AddValue(type, dtList, dateTimeVal);
                return;
            }

            list.Add(strVal); // assumes List<object>
        }

        private void AddValue(TokenTypeEnum type, List<double> list, double dblVal)
        {
            Contract.Assert(type == TokenTypeEnum.Missing || type == TokenTypeEnum.Double);
            list.Add(type == TokenTypeEnum.Missing ? double.NaN : dblVal);
        }

        private void AddValue(TokenTypeEnum type, List<string> list, string strVal)
        {
            // assumes that strVal is always set to the original token read from the input file
            list.Add(type == TokenTypeEnum.Missing ? string.Empty : strVal);
        }

        private void AddValue(TokenTypeEnum type, List<DateTime> list, DateTime dtVal)
        {
            Contract.Assert(type == TokenTypeEnum.Missing || type == TokenTypeEnum.DateTime);
            list.Add(type == TokenTypeEnum.Missing ? DateTime.MinValue : dtVal);
        }

        private IList CreateList(TokenTypeEnum type, int estimatedNumberOfLines)
        {
            switch (type)
            {
                case TokenTypeEnum.String:
                    return new List<string>(estimatedNumberOfLines);
                case TokenTypeEnum.Double:
                    return new List<double>(estimatedNumberOfLines);
                case TokenTypeEnum.Missing: // assume double columns
                    return new List<double>(estimatedNumberOfLines);
                case TokenTypeEnum.DateTime:
                    return new List<DateTime>(estimatedNumberOfLines);
                default:
                    throw new InvalidOperationException();
            }
        }

        #endregion

        #region tokenizer

        // the tokenizer reads full lines and returns separated tokens in the line as well as a terminating end-of-line character
        internal enum TokenTypeEnum
        {
            NewLine,
            String,
            Double,
            DateTime,
            Missing
        }

        internal class Tokenizer
        {
            private readonly char[] _separators;

            // arrays for string.Split()
            private readonly char[] _whiteSpaceSeparators = new char[0]; // string split uses separators as default

            private readonly DateTimeFormatInfo _dateTimeFormatInfo;
            private DateTime[] _dateTimeVals = new DateTime[1024];
            private double[] _doubleVals = new double[1024];
            private readonly NumberFormatInfo _numberFormatInfo;
            private int _numTokens;
            private readonly StreamReader _reader;
            private readonly char _separator;
            private string[] _stringVals = new string[1024];
            private int _tokenPos;
            // we assume that a buffer of 1024 tokens for a line is sufficient most of the time (the buffer is increased below if necessary)
            private TokenTypeEnum[] _tokenTypes = new TokenTypeEnum[1024];

            public Tokenizer(StreamReader reader, NumberFormatInfo numberFormatInfo,
                DateTimeFormatInfo dateTimeFormatInfo, char separator)
            {
                _reader = reader;
                _numberFormatInfo = numberFormatInfo;
                _dateTimeFormatInfo = dateTimeFormatInfo;
                _separator = separator;
                _separators = new[] { separator };
                ReadNextTokens();
            }

            public int CurrentLineNumber { get; private set; }

            public string CurrentLine { get; private set; }

            public long BytesRead { get; private set; }

            public bool HasNext()
            {
                return _numTokens > _tokenPos || !_reader.EndOfStream;
            }

            public TokenTypeEnum PeekType()
            {
                return _tokenTypes[_tokenPos];
            }

            public void Skip()
            {
                // simply skips one token without returning the result values
                _tokenPos++;
                if (_numTokens == _tokenPos)
                {
                    ReadNextTokens();
                }
            }

            public void Next(out TokenTypeEnum type, out string strVal, out double dblVal, out DateTime dateTimeVal)
            {
                type = _tokenTypes[_tokenPos];
                strVal = _stringVals[_tokenPos];
                dblVal = _doubleVals[_tokenPos];
                dateTimeVal = _dateTimeVals[_tokenPos];
                Skip();
            }

            private void ReadNextTokens()
            {
                if (!_reader.EndOfStream)
                {
                    CurrentLine = _reader.ReadLine();
                    CurrentLineNumber++;
                    if (_reader.BaseStream.CanSeek)
                    {
                        BytesRead = _reader.BaseStream.Position;
                    }
                    else
                    {
                        if (CurrentLine != null) BytesRead += CurrentLine.Length + 2; // guess
                    }
                    var i = 0;
                    if (!string.IsNullOrWhiteSpace(CurrentLine))
                    {
                        foreach (var tok in Split(CurrentLine))
                        {
                            TokenTypeEnum type;
                            double doubleVal;
                            DateTime dateTimeValue;
                            type = TokenTypeEnum.String; // default
                            _stringVals[i] = tok.Trim();
                            if (double.TryParse(tok, NumberStyles.Float, _numberFormatInfo, out doubleVal))
                            {
                                type = TokenTypeEnum.Double;
                                _doubleVals[i] = doubleVal;
                            }
                            else if (DateTime.TryParse(tok, _dateTimeFormatInfo, DateTimeStyles.None, out dateTimeValue))
                            {
                                type = TokenTypeEnum.DateTime;
                                _dateTimeVals[i] = dateTimeValue;
                            }
                            else if (string.IsNullOrWhiteSpace(tok))
                            {
                                type = TokenTypeEnum.Missing;
                            }

                            // couldn't parse the token as an int or float number or datetime value so return a string token

                            _tokenTypes[i] = type;
                            i++;

                            if (i >= _tokenTypes.Length)
                            {
                                // increase buffer size if necessary
                                IncreaseCapacity(ref _tokenTypes);
                                IncreaseCapacity(ref _doubleVals);
                                IncreaseCapacity(ref _stringVals);
                                IncreaseCapacity(ref _dateTimeVals);
                            }
                        }
                    }
                    _tokenTypes[i] = TokenTypeEnum.NewLine;
                    _numTokens = i + 1;
                    _tokenPos = 0;
                }
            }

            private IEnumerable<string> Split(string line)
            {
                return _separator == Whitespacechar
                    ? line.Split(_whiteSpaceSeparators, StringSplitOptions.RemoveEmptyEntries)
                    : line.Split(_separators);
            }

            private static void IncreaseCapacity<T>(ref T[] arr)
            {
                var n = (int)Math.Floor(arr.Length * 1.7); // guess
                var arr2 = new T[n];
                Array.Copy(arr, arr2, arr.Length);
                arr = arr2;
            }
        }

        #endregion

        #region parsing

        private void ParseVariableNames()
        {
            // the first line must contain variable names
            var varNames = new List<string>();

            TokenTypeEnum type;
            string strVal;
            double dblVal;
            DateTime dateTimeVal;

            _tokenizer.Next(out type, out strVal, out dblVal, out dateTimeVal);

            // the first token must be a variable name
            if (type != TokenTypeEnum.String)
                throw new ArgumentException("Error: Expected " + TokenTypeEnum.String + " got " + type);
            varNames.Add(strVal);

            while (_tokenizer.HasNext() && _tokenizer.PeekType() != TokenTypeEnum.NewLine)
            {
                _tokenizer.Next(out type, out strVal, out dblVal, out dateTimeVal);
                varNames.Add(strVal);
            }
            ExpectType(TokenTypeEnum.NewLine);

            _variableNames = varNames;
        }

        private void ExpectType(TokenTypeEnum expectedToken)
        {
            if (_tokenizer.PeekType() != expectedToken)
                throw new ArgumentException("Error: Expected " + expectedToken + " got " + _tokenizer.PeekType());
            _tokenizer.Skip();
        }

        private void Error(string message, string token, int lineNumber)
        {
            throw new DataFormatException("Error while parsing.\n" + message, token, lineNumber);
        }

        #endregion
    }

}