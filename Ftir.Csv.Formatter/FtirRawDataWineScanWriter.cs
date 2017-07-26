using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Ftir.Csv.Formatter
{
    /// <summary>
    /// Implements the <see cref="FtirRawDataWriteBase"/> functionality such like <see cref="Write"/>
    /// to format a "Wine Scan" data file to *.csv format
    /// </summary>
    class FtirRawDataWineScanWriter : FtirRawDataWriteBase
    {
        private readonly FileInfo _fileInfo;
        private readonly bool _toMyLocalesFormat;
        private readonly string[] _columns;

        /// <summary>
        /// Creates an instance of <see cref="FtirRawDataWineScanWriter"/> initialized with the
        /// passed parameters
        /// </summary>
        /// <param name="fileName">Pass the file name to write the result data to</param>
        /// <param name="toMyLocalesFormat">Pass the translate to my number style locales settings (. to , and , to ;)</param>
        /// <param name="columns">Pass the columns to write to file. If passed null all columns are written</param>
        public FtirRawDataWineScanWriter(string fileName, bool toMyLocalesFormat, string[] columns)
        {
            _toMyLocalesFormat = toMyLocalesFormat;
            _fileInfo = new FileInfo(fileName);
            _columns = columns;

            EnsureFileExists(_fileInfo);
        }

        /// <summary>
        /// Writes a set of <see cref="FtirData"/> formated to file
        /// </summary>
        /// <param name="items">Pass a set of <see cref="FtirData"/> that needs to be written to file</param>
        public override void Write(FtirData items)
        {
            EnsureFileExists(_fileInfo);

            if (_columns != null)
            {
                items = StripColumns(items, _columns);
            }

            using (FileStream fStm = _fileInfo.Create())
            using (StreamWriter sStm = new StreamWriter(fStm, Encoding.Default))
            {
                var data = Stringify(items.Header.Tuple, items.SeparatorChar, items.DecimalChar, _toMyLocalesFormat);
                sStm.WriteLine(data);

                foreach (FtirItem item in items.Data)
                {
                    var val = item.Tuple.LastOrDefault(i => i.ToLowerInvariant() == "value");
                    if (val == null)
                    {
                        continue;
                    }

                    data = Stringify(item.Tuple, items.SeparatorChar, items.DecimalChar, _toMyLocalesFormat);
                    sStm.WriteLine(data);
                }
            }
        }
    }
}