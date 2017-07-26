using CommandLine;
using CommandLine.Text;

namespace Ftir.Csv.Formatter
{
    class ShellCommands
    {
        [OptionArray('f', "files", HelpText = "Formats a set of data files into to valid csv files. Examle: Ftir.Csv.Formatter.exe -a \"C:\\temp\\160802_GrapeScan P30.csv\" \"C:\\temp\\160802_GrapeScan P31.csv\" \"C:\\temp\\160802_GrapeScan P32.csv\"")]
        public string[] Files { get; set; }

        [Option('d',"directory",HelpText = "Formats all files within a directory to valid csv files. Example Ftir.Csv.Formatter.exe -d \"C:\\temp\\160802_GrapeScan\"")]
        public string Directory { get; set; }

        [Option('k', "myLocales", HelpText = "Changes the separator char and the numerical separator my locales (e.g '.' to ',')", DefaultValue = false)]
        public bool ToMyLocalFormat { get; set; }

        [OptionArray('c', "columns", HelpText = "Restrict output data to columns. Example: \"Probe\" \"F-SO2\"")]
        public string[] Columns { get; set; }

        [HelpOption(HelpText = "Dispaly this help screen.")]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this);
        }
    }
}