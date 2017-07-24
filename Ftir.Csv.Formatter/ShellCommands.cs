using CommandLine;
using CommandLine.Text;

namespace Ftir.Csv.Formatter
{
    public class ShellCommands
    {
        [OptionArray('f', "files", HelpText = "Formats a set of data files into to valid csv files. Examle: Ftir.Csv.Formatter.exe -a \"C:\\temp\\160802_GrapeScan P30.csv\" \"C:\\temp\\160802_GrapeScan P31.csv\" \"C:\\temp\\160802_GrapeScan P32.csv\"")]
        public string[] Files { get; set; }

        [Option('d',"directory",HelpText = "Formats all files within a directory to valid csv files. Example Ftir.Csv.Formatter.exe -d \"C:\\temp\\160802_GrapeScan\"")]
        public string Directory { get; set; }

        [Option('k', "keepFormat", HelpText = "Keeps the csv format for decimal and seperator char", DefaultValue = true)]
        public bool KeepCsvFormat { get; set; }

        [HelpOption(HelpText = "Dispaly this help screen.")]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this);
        }
    }
}