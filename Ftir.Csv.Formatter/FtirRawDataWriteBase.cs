using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Ftir.Csv.Formatter
{
    /// <summary>
    /// Defines the abstract for <see cref="FtirRawDataWriteBase"/> such like <see cref="Write"/>
    /// or <see cref="StripColumns"/>
    /// </summary>
    internal abstract class FtirRawDataWriteBase
    {
        /// <summary>
        /// Writes a set of <see cref="FtirData"/> to target
        /// </summary>
        /// <param name="items">Pass the <see cref="FtirData"/> that needs to be written to target</param>
        /// <exception cref="FileNotFoundException"></exception>        
        public abstract void Write(FtirData items);

        /// <summary>
        /// Strips a set of <see cref="FtirData"/> accordingt to the passed columns
        /// </summary>
        /// <param name="data">Pass the data to be stripped</param>
        /// <param name="columns">Pass th ecolumsn to be selected for writing</param>
        /// <returns>returns a stripped set of <see cref="FtirData"/></returns>
        protected FtirData StripColumns(FtirData data, string[] columns)
        {

            List<string> headerFiltered = new List<string>();
            bool[] selectedColums = new bool[data.Header.Tuple.Length];

            for (int i = 0; i < data.Header.Tuple.Length; i++)
            {
                var col = data.Header.Tuple[i];

                if (columns.Any(t => t.ToLowerInvariant() == col.ToLowerInvariant()))
                {
                    headerFiltered.Add(col);
                    selectedColums[i] = true;
                }
                else
                {
                    selectedColums[i] = false;
                }
            }

            List<FtirItem> selectedItems = new List<FtirItem>();
            foreach (FtirItem item in data.Data)
            {

                List<string> items = new List<string>();
                for (int i = 0; i < item.Tuple.Length; i++)
                {
                    if (selectedColums[i])
                    {
                        items.Add(item.Tuple[i]);
                    }
                }

                selectedItems.Add(new FtirItem { Tuple = items.ToArray() });
            }

            return new FtirData
            {
                Header = new FtirItem { Tuple = headerFiltered.ToArray() },
                Data = selectedItems.ToArray(),
                DecimalChar = data.DecimalChar,
                SeparatorChar = data.SeparatorChar,
            };
        }

        /// <summary>
        /// Gets the to be written item string according to the writer settings
        /// </summary>
        /// <param name="items">Pass the items that needs to be written to a single line in csv</param>
        /// <param name="sepChar">Pass the seperator character to be used for the items</param>
        /// <param name="decChar">Pass the decimal number character to be used fo <see cref="float"/> or <see cref="double"/></param>
        /// <param name="toLocalNumberStyle">Pass the number syle flag to local</param>
        /// <returns>Returns a linearized stirng according ot the passed parameters</returns>
        protected string Stringify(string[] items, char sepChar, char decChar, bool toLocalNumberStyle)
        {
            StringBuilder b = new StringBuilder();

            foreach (string item in items)
            {
                if (b.Length > 0 || string.IsNullOrEmpty(item))
                {
                    if (toLocalNumberStyle)
                    {
                        b.Append(GetSeparatorChar(CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator));
                    }
                    else
                    {
                        b.Append(sepChar);
                    }
                }

                if (!string.IsNullOrEmpty(item))
                {
                    var replaced = item;
                    if (toLocalNumberStyle)
                    {
                        replaced = replaced.Replace($"{decChar}", CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator);
                    }

                    b.Append(replaced);
                }
            }

            return b.ToString();
        }

        private char GetSeparatorChar(string c)
        {
            if (c == ".")
            {
                return ',';
            }

            return ';';
        }

        /// <summary>
        /// Checks if a file exists and is able to be accessed
        /// </summary>
        /// <param name="fi">pass the <see cref="FileInfo"/> to be evaluated</param>
        /// <exception cref="FileNotFoundException"></exception>
        protected void EnsureFileExists(FileInfo fi)
        {
            if (fi == null || fi.Exists)
            {
                throw new AccessViolationException($"The file {fi?.FullName} already exists. Check file name or file exists");
            }
        }


    }
}