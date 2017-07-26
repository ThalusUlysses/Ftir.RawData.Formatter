using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Ftir.Csv.Formatter
{
    /// <summary>
    /// Implements <see cref="FtirRawDataReader"/> such like <see cref="Read"/> FTIR
    /// *.csv raw data
    /// </summary>
    class FtirRawDataReader
    {
        private readonly FileInfo _fileInfo;

        /// <summary>
        /// Creates an instance of <see cref="FtirRawDataReader"/> with the passed parameters
        /// </summary>
        /// <param name="fileName">Pass the file name to read the unformatted data from</param>
        public FtirRawDataReader(string fileName)
        {
            _fileInfo = new FileInfo(fileName);
            EnsureFileExists();
        }

        private char DetectSeparatorCharFromFile(string st)
        {
            int countColon = 0;
            int countComma =0;
            foreach (char c in st)
            {
                if (c == ';')
                {
                    countColon++;
                }

                if (c == ',')
                {
                    countComma++;
                }
            }

            if (countComma == 0 && countColon == 0)
            {
                return '{';
            }

            if (countComma > countColon)
            {
                return ',';
            }

            return ';';
        }

        private char DetectDecimalCharFromFile(string st)
        {
            int countDot = 0;
            int countComma = 0;
            foreach (char c in st)
            {
                if (c == '.')
                {
                    countDot++;
                }

                if (c == ',')
                {
                    countComma++;
                }
            }

            if (countComma == 0 && countDot == 0)
            {
                return '{';
            }

            if (countComma > countDot)
            {
                return ',';
            }

            return '.';
        }

        /// <summary>
        /// Reads a set of <see cref="FtirData"/> from a specified file
        /// </summary>
        /// <returns>Returns a set of <see cref="FtirData"/> filled with the read parameters</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public FtirData Read()
        {
            EnsureFileExists();
            FtirItem header = null;
            List<FtirItem> sampleResults = new List<FtirItem>();

            char sepChar = '{';
            char decChar = '{';

            using (FileStream fStm = _fileInfo.OpenRead())
            using (StreamReader sStm = new StreamReader(fStm, System.Text.Encoding.Default))
            {
                while (sStm.Peek() > 0)
                {
                    var theLine = sStm.ReadLine();
                    
                    if (string.IsNullOrEmpty(theLine))
                    {
                        continue;
                    }

                    var invLower = theLine.ToLowerInvariant();

                    if (invLower.StartsWith("job name") || invLower.StartsWith("collection date") ||
                        invLower.StartsWith("job type") || invLower.StartsWith($"{sepChar}{sepChar}"))
                    {
                        continue;
                    }

                    if (sepChar == '{')
                    {
                        sepChar = DetectSeparatorCharFromFile(theLine);
                    }

                    var items = theLine.Split(sepChar);

                    if (invLower.StartsWith($"probe{sepChar}"))
                    {
                        if (header == null)
                        {
                            header = new FtirItem {Tuple = items};
                        }
                        continue;
                    }

                    if (decChar == '{')
                    {
                        decChar = DetectDecimalCharFromFile(theLine);
                    }

                    FtirItem item = new FtirItem
                    {
                        Tuple = items
                    };

                    sampleResults.Add(item);
                }
            }

            return new FtirData
            {
                Data = sampleResults.ToArray(),
                SeparatorChar = sepChar,
                DecimalChar = decChar,
                Header = header
            };
        }

        private void EnsureFileExists()
        {
            if (!_fileInfo.Exists)
            {
                throw new FileNotFoundException($"The file {_fileInfo.FullName} could not be found. Check file name or file exists");
            }
        }
    }
}