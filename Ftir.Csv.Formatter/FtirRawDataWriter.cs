using System.Globalization;
using System.IO;
using System.Text;

namespace Ftir.Csv.Formatter
{
    class FtirRawDataWriter
    {
        private FileInfo _fileInfo;
        private bool _keepPassedFromat;

        public FtirRawDataWriter(string fileName, bool keepPassedFromat)
        {
            _keepPassedFromat = keepPassedFromat;
            _fileInfo = new FileInfo(fileName);
            EnsureFileExists();
        }

        public void Write(FtirData items)
        {
            EnsureFileExists();

            using (FileStream fStm = _fileInfo.Create())
            using (StreamWriter sStm = new StreamWriter(fStm, Encoding.Default))
            {
                foreach (FtirItem item in items.Data)
                {
                    var data = GetItemString(item.Tuple, items.SeparatorChar, items.DecimalChar);
                    sStm.WriteLine(data);
                }
            }
        }

        private string GetItemString(string[] items,char sepChar, char decChar)
        {
            StringBuilder b = new StringBuilder();

            foreach (string item in items)
            {
                if (b.Length > 0 || string.IsNullOrEmpty(item))
                {
                    if (_keepPassedFromat)
                    {
                        b.Append(sepChar);
                    }
                    else
                    {
                        b.Append(GetSeparatorChar(decChar));
                    }
                }

                if (!string.IsNullOrEmpty(item))
                {
                    var replaced = item;
                    if (!_keepPassedFromat)
                    {
                        replaced = replaced.Replace($"{decChar}",CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator);
                    }

                    b.Append(replaced);
                }
            }

            return b.ToString();
        }

        private char GetSeparatorChar(char c)
        {
            if (c == '.')
            {
                return ',';
            }

            return ';';
        }

        private void EnsureFileExists()
        {
            if (_fileInfo.Exists)
            {
                throw new FileNotFoundException($"The file {_fileInfo.FullName} already exists. Check file name or file exists");
            }
        }
    }
}