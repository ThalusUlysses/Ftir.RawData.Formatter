using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Ftir.Csv.Formatter
{
    class FtirRawDataReader
    {
        private FileInfo _fileInfo;

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
                if (c == ';')
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

        public FtirData Read()
        {
            EnsureFileExists();
            bool nextIsData = false;
            FtirItem header = null;
            List<FtirItem> sampleResults = new List<FtirItem>();

            char sepChar = '{';
            char decChar = '{';

            using (FileStream fStm = _fileInfo.OpenRead())
            using (StreamReader sStm = new StreamReader(fStm, System.Text.Encoding.Default))
            {
                List<string> headerArgs = new List<string>();
                List<string> evalArgs = new List<string>();
                while (sStm.Peek() > 0)
                {
                    var theLine = sStm.ReadLine();

                    if (sepChar == '{')
                    {
                        sepChar = DetectSeparatorCharFromFile(theLine);
                    }

                    if (string.IsNullOrEmpty(theLine))
                    {
                        continue;
                    }

                    var invLower = theLine.ToLowerInvariant();

                    if (invLower.StartsWith("job name"))
                    {
                        int idx = invLower.IndexOf(':');

                        headerArgs.Add(theLine.Substring(0,idx));
                        evalArgs.Add(theLine.Substring(idx + 1).Trim());
                        continue;
                    }

                    if (invLower.StartsWith("collection date"))
                    {
                        int idx = invLower.IndexOf(':');

                        headerArgs.Add(theLine.Substring(0, idx));
                        evalArgs.Add(theLine.Substring(idx + 1).Trim());
                        continue;
                    }


                    if (invLower.StartsWith("job type"))
                    {
                        int idx = invLower.IndexOf(':');

                        headerArgs.Add(theLine.Substring(0, idx));
                        evalArgs.Add(theLine.Substring(idx + 1).Trim());
                        continue;
                    }

                    if (invLower.StartsWith($"probe{sepChar}"))
                    {
                        var items = theLine.Split(sepChar);
                        headerArgs.AddRange(items);
                        nextIsData = true;
                        continue;
                    }

                    if (invLower.StartsWith($"{sepChar}{sepChar}"))
                    {
                        evalArgs.AddRange(new[] { string.Empty, string.Empty});
                        nextIsData = true;
                    }

                    if (nextIsData)
                    {
                        if (header == null)
                        {
                            header = new FtirItem
                            {
                                Tuple = headerArgs.ToArray()
                            };

                            sampleResults.Add(header);
                        }


                        if (decChar == '{')
                        {
                            decChar = DetectDecimalCharFromFile(theLine);
                        }

                        var items = theLine.Split(sepChar);

                        FtirItem item = new FtirItem
                        {
                            Tuple = evalArgs.Concat(items).ToArray()
                        };

                        evalArgs = new List<string>();

                        sampleResults.Add(item);

                        nextIsData = false;
                    }
                }
            }

            return new FtirData
            {
                Data = sampleResults.ToArray(),
                SeparatorChar = sepChar,
                DecimalChar = decChar
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