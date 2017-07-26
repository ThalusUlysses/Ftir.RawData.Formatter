namespace Ftir.Csv.Formatter
{
    class FtirData
    {
        public char SeparatorChar { get; set; }
        public char DecimalChar { get; set; }
        public FtirItem[] Data { get; set; }

        public FtirItem Header { get; set; }
    }
}