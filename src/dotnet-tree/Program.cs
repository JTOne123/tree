using System;
using System.IO;
using System.Text;

namespace dotnet_tree
{
    public class Program
    {
        private static bool _isOutputRedirected = Console.IsOutputRedirected;
        private static int _bufferWidth = Console.BufferWidth;
        private static int _depth;
        private static int _line;

        public static int Main(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "-v":
                    case "--version":
                        Console.WriteLine("0.2.2");
                        return 0;
                    case "-h":
                    case "--help":
                        PrintHelp();
                        return 0;
                    default:
                        PrintHelp();
                        return 1;
                }
            }

            PrintDirectoryTree();
            return 0;
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Usage: dotnet-tree [options]");
            Console.WriteLine("Options");
            Console.WriteLine("  -v --version                      Print version.");
            Console.WriteLine("  -h --help                         Print usage.");
        }

        private static void PrintDirectoryTree()
        {
            Console.WriteLine(".");
            PrintDirectory(".");
        }

        private static void PrintDirectory(string directory)
        {
            if (_depth > 15)
            {
                throw new Exception("The path is too deep and supports up to 15 layers of paths.");
            }

            FileSystemInfo[] fileSystemInfo = new DirectoryInfo(directory).GetFileSystemInfos();
            ++_depth;
            for (int i = 0; i < fileSystemInfo.Length; i++)
            {
                int index = fileSystemInfo.Length - 1;
                string formatValue = string.Format(
                    "{0} {1}",
                    i == index ? "\\---" : "|--=",
                    fileSystemInfo[i].Name);

                PrintName(formatValue);

                if (fileSystemInfo[i].Attributes.HasFlag(FileAttributes.Directory))
                {
                    if (i == index)
                    {
                        PrintDirectory(fileSystemInfo[i].FullName);
                    }
                    else
                    {
                        _line ^= 1 << _depth;
                        PrintDirectory(fileSystemInfo[i].FullName);
                        _line ^= 1 << _depth;
                    }
                }
            }
            --_depth;
        }

        private static void PrintName(string name)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 1; i < _depth; i++)
            {
                int value = (_line >> i) & 1;
                if (value == 1)
                {
                    stringBuilder.Append("| ");
                }
                else
                {
                    stringBuilder.Append("  ");
                }
            }

            stringBuilder.Append(name);
            name = stringBuilder.ToString();

            if (_isOutputRedirected)
            {
                Console.WriteLine(name);
                return;
            }

            Console.WriteLine(name.Length > _bufferWidth ? name.Substring(0, _bufferWidth) : name);
        }
    }
}
