using System;
using System.Linq;

namespace Ftir.Csv.Formatter
{
    /// <summary>
    /// Implements wrapper funtionality to hide a set of writers behind
    /// such like <see cref="FtirRawDataWineScanWriter"/>
    /// </summary>
    class FtirRawDataWriterWrapper
    {
        readonly bool _toMyLocalesFormat;
        readonly string _fileInfo;
        private readonly string[] _columns;

        /// <summary>
        /// Creates an instance of <see cref="FtirRawDataWriterWrapper"/> with th epassed parameters
        /// </summary>
        /// <param name="fileName">Pass the file nam eto write the result to</param>
        /// <param name="toMyLocalesFormat">Pass the transfer to my local decimal number formate</param>
        /// <param name="columns">Pass the columns to write. If nul is passed no filtering is performed</param>
        public FtirRawDataWriterWrapper(string fileName, bool toMyLocalesFormat, string[] columns)
        {
            _toMyLocalesFormat = toMyLocalesFormat;
            _columns = columns;
            _fileInfo = fileName;
        }

        /// <summary>
        /// Writes a set of <see cref="FtirData"/> to file considering local decimal settings
        /// or passed ones
        /// </summary>
        /// <param name="items">Pass a data ited that need needs to be written to file</param>
        public void Write(FtirData items)
        {
            FtirRawDataWriteBase writer = IsWineScanData(items)
                ? (FtirRawDataWriteBase) new FtirRawDataWineScanWriter(_fileInfo, _toMyLocalesFormat, _columns)
                : new FtirRawDataGrapeScanWriter(_fileInfo, _toMyLocalesFormat, _columns);

            writer.Write(items);
        }

        bool IsWineScanData(FtirData items)
        {
			
			if (items.Header.Tuple.Any(i => i.ToLowerInvariant() == "f-so2"))
			{
				return true;
			}

			var idx = GetIndexOfColumn("produkt", items);

			return items.Data.Select(i => i.Tuple[idx]).Any(i => i.ToLowerInvariant().Contains("wein"));
		}

		int GetIndexOfColumn(string name,FtirData items)
		{
			int idx = 0;
			for (int i = 0; i < items.Header.Tuple.Length; i++)
			{
				if (items.Header.Tuple[i].ToLowerInvariant() == name)
				{
					return i;
				}
			}

			throw new ArgumentException($"Invalid column name {name}");
		}
    }
}