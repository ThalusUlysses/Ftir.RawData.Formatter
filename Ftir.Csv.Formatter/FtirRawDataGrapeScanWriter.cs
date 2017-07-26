using System.Globalization;
using System.IO;
using System.Text;

namespace Ftir.Csv.Formatter
{
    /// <summary>
    /// Implements writer funtionality of <see cref="FtirRawDataWriteBase"/> to write
    /// "Grape Scan" data formatted to a specified file
    /// </summary>
    class FtirRawDataGrapeScanWriter : FtirRawDataWriteBase
    {
        private readonly FileInfo _fileInfo;
        private readonly bool _toMyLocalesFormat;
        private readonly string[] _columns;

        /// <summary>
        /// Creates an instance of <see cref="FtirRawDataGrapeScanWriter"/> with the passed parameters
        /// </summary>
        /// <param name="fileName">Pass the file name to write the result to</param>
        /// <param name="toMyLocalesFormat">Pass the translate data to my local decimal number sytle (. to , and , to ;)</param>
        /// <param name="columns">Pass the columns to be written to file. If passed null all columns are written</param>
        public FtirRawDataGrapeScanWriter(string fileName, bool toMyLocalesFormat, string[] columns)
        {
            _toMyLocalesFormat = toMyLocalesFormat;
            _fileInfo = new FileInfo(fileName);
            _columns = columns;
            EnsureFileExists(_fileInfo);
        }

        /// <summary>
        /// Writes a set of <see cref="FtirData"/> formatted to specified file
        /// </summary>
        /// <param name="items">pas the <see cref="FtirData"/> that needs to be formatted and written to file</param>
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
                    data = Stringify(item.Tuple, items.SeparatorChar, items.DecimalChar, _toMyLocalesFormat);
                    sStm.WriteLine(data);
                }
            }
        }
    }
}