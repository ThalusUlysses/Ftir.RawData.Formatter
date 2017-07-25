using System;
using System.IO;
using System.Linq;

namespace Ftir.Csv.Formatter
{
    class Program
    {
        static void Main(string[] args)
        {
            bool isInteractive = !args.Any();
            bool isJustHelp = false;
            try
            {
                ShellCommands shellCmds = new ShellCommands();
                if (!CommandLine.Parser.Default.ParseArguments(args, shellCmds))
                {
                    if (args.Length == 0)
                    {
                        shellCmds.Files =
                            Directory.GetFiles(Environment.CurrentDirectory, "*.csv");
                    }
                    else if(args.Length == 1 && args[0] == "-?")
                    {
                        isJustHelp = true;
                        isInteractive = true;
                    }
                    else
                    {
                        shellCmds.Files = args;
                    }
                }

                if (!isJustHelp)
                {
                    Evaluate(shellCmds);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.ExitCode = -1;
            }

            if (isInteractive)
            {
                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        private static void Evaluate(ShellCommands shellCmds)
        {
            if (shellCmds.Directory != null)
            {
                shellCmds.Files = Directory.GetFiles(shellCmds.Directory, "*.csv");
            }

            if (shellCmds.Files == null)
            {
                shellCmds.Files = Directory.GetFiles(Environment.CurrentDirectory, "*.csv");
            }


            foreach (string s in shellCmds.Files)
            {
                var items = Read(s);

                FileInfo fi = new FileInfo(s);
                string path = EnsureOutDirectory(fi.DirectoryName);

                path = Path.Combine(path, fi.Name);
                Write(items, path, shellCmds.ToMyLocalFormat);
            }
        }

        private static FtirData Read(string s)
        {
            Console.Write($"Read data from {s}");

            FtirRawDataReader reader = new FtirRawDataReader(s);

            var items = reader.Read();

            Console.WriteLine($"...OK");
            return items;
        }

        private static string EnsureOutDirectory(string root)
        {
            var t = Path.Combine(root, "Formatted");

            if (!Directory.Exists(t))
            {
                Console.Write($"Create output directory {t}");

                Directory.CreateDirectory(t);

                Console.WriteLine("...OK");
            }

            return t;
        }

        private static void  Write(FtirData s, string name, bool toMylocales)
        {
            Console.Write($"Write output to file  output directory {name}");

            FtirRawDataWriter wrt = new FtirRawDataWriter(name, toMylocales);

            wrt.Write(s);

            Console.WriteLine("...OK");
        }
    }
}
