using System.IO;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace Ftir.Csv.Formatter
{
    class FtirRawDataWriter
    {
        private FileInfo _fileInfo;
        private string _sepChar = ",";

        public FtirRawDataWriter(string fileName)
        {
            _fileInfo = new FileInfo(fileName);
            EnsureFileExists();
        }

        public void Write(FtirItem[] items)
        {
            EnsureFileExists();

            using (FileStream fStm = _fileInfo.Create())
            using (StreamWriter sStm = new StreamWriter(fStm, Encoding.Default))
            {
                foreach (FtirItem item in items)
                {
                    var data = GetItemString(item.Tuple);
                    sStm.WriteLine(data);
                }
            }
        }

        private string GetItemString(string[] items)
        {
            StringBuilder b = new StringBuilder();

            foreach (string item in items)
            {
                if (b.Length > 0 || string.IsNullOrEmpty(item))
                {
                    b.Append(_sepChar);
                }

                if (!string.IsNullOrEmpty(item))
                {
                    b.Append(item.Replace(',', '.'));
                }
            }

            return b.ToString();
        }

        private void EnsureFileExists()
        {
            if (_fileInfo.Exists)
            {
                throw new FileNotFoundException($"The file {_fileInfo.FullName} already exists. Check file name or file exists");
            }
        }
    }

    public class ShellCommands
    {
        [OptionArray('f', "files", HelpText = "Formats a set of data files into to valid csv files. Examle: Ftir.Csv.Formatter.exe -a \"C:\\temp\\160802_GrapeScan P30.csv\" \"C:\\temp\\160802_GrapeScan P31.csv\" \"C:\\temp\\160802_GrapeScan P32.csv\"")]
        public string[] Files { get; set; }

        [Option('d',"directory",HelpText = "Formats all files within a directory to valid csv files. Example Ftir.Csv.Formatter.exe -d \"C:\\temp\\160802_GrapeScan\"")]
        public string Directory { get; set; }

        [HelpOption(HelpText = "Dispaly this help screen.")]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this);
        }
    }
}