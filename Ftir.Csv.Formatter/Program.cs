using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Ftir.Csv.Formatter
{
    class Program
    {
        private static bool _isInteractive;
        static bool _isJustHelp = false;
        static void Main(string[] args)
        {
            ShellCommands shCmds = null;
            try
            {
                _isInteractive = !args.Any() || Debugger.IsAttached;

                if (_isInteractive)
                {
                    PrintWelcomeMessage();
                }

                shCmds =  ParseShellCommands(args);

                AdjustShellCommandData(shCmds);

                if (!_isJustHelp)
                {
                    ProcessShellCommands(shCmds);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                if (shCmds != null)
                {
                    Console.WriteLine(shCmds.GetUsage());
                }

                Environment.ExitCode = -1;
            }

            if (_isInteractive)
            {
                PrintGoodByMessage(Environment.ExitCode == 0);
            }
        }

        private static void PrintWelcomeMessage()
        {
            var asmName = Assembly.GetAssembly(typeof(Program)).GetName().FullName;
            Console.WriteLine($"-----------------------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine($"Started {asmName} in interactive mode");
            Console.WriteLine();
        }

        private static void PrintGoodByMessage(bool successFull)
        {
            if (successFull)
            {
                Console.WriteLine();
                var asmName = Assembly.GetAssembly(typeof(Program)).GetName().FullName;
                Console.WriteLine($"{asmName} ended successfully.");
            }

            Console.WriteLine();
            Console.WriteLine($"-----------------------------------------------------------------------------");
            Console.WriteLine();

            Console.WriteLine("Press any key to exit...");
            
            Console.ReadKey();
        }

        private static void AdjustShellCommandData(ShellCommands shCmds)
        {
            if (shCmds.Directory != null)
            {
                shCmds.Files = Directory.GetFiles(shCmds.Directory, "*.csv");
            }

            if (shCmds.Files == null)
            {
                shCmds.Files = Directory.GetFiles(Environment.CurrentDirectory, "*.csv");
            }
        }

        private static ShellCommands ParseShellCommands(string [] args)
        {
            ShellCommands shellCmds = new ShellCommands();

            if (!CommandLine.Parser.Default.ParseArguments(args, shellCmds))
            {
                if (args.Length == 0)
                {
                    shellCmds.Files =
                        Directory.GetFiles(Environment.CurrentDirectory, "*.csv");
                }
                else if (args.Length == 1 && args[0] == "-?")
                {
                    _isJustHelp = true;
                    _isInteractive = true;
                }
                else
                {
                    shellCmds.Files = args;
                }
            }

            return shellCmds;
        }

        private static void ProcessShellCommands(ShellCommands shellCmds)
        {
            foreach (string s in shellCmds.Files)
            {
                var items = Read(s);

                FileInfo fi = new FileInfo(s);
                string path = EnsureOutDirectory(fi.DirectoryName);

                path = Path.Combine(path, fi.Name);
                Write(items, path, shellCmds.ToMyLocalFormat, shellCmds.Columns);
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

        private static void  Write(FtirData s, string name, bool toMylocales, string[] filteredColumns)
        {
            Console.Write($"Write output to file  output directory {name}");

            FtirRawDataWriterWrapper wrt = new FtirRawDataWriterWrapper(name, toMylocales, filteredColumns);

            wrt.Write(s);

            Console.WriteLine("...OK");
        }
    }
};