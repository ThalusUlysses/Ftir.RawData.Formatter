using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ftir.Csv.Formatter
{
    class FtirRawDataReader
    {
        private FileInfo _fileInfo;
        private char _sepChar = ';';

        public FtirRawDataReader(string fileName)
        {
            _fileInfo = new FileInfo(fileName);
            EnsureFileExists();
        }

        public FtirItem[] Read()
        {
            EnsureFileExists();
            bool nextIsData = false;
            FtirItem header = null;
            List<FtirItem> sampleResults = new List<FtirItem>();

            using (FileStream fStm = _fileInfo.OpenRead())
            using (StreamReader sStm = new StreamReader(fStm, System.Text.Encoding.Default))
            {
                List<string> headerArgs = new List<string>();
                List<string> evalArgs = new List<string>();
                while (sStm.Peek() > 0)
                {
                    var theLine = sStm.ReadLine();
                    
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

                    if (invLower.StartsWith("probe;"))
                    {
                        var items = theLine.Split(_sepChar);
                        headerArgs.AddRange(items);
                        nextIsData = true;
                        continue;
                    }

                    if (invLower.StartsWith(";;"))
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

                        var items = theLine.Split(_sepChar);

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
            return sampleResults.ToArray();
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