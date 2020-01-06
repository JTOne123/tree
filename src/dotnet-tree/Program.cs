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
                if (args[0].Equals("-v") || args[0].Equals("--version"))
                {
                    Console.WriteLine("0.2.1");
                    return 0;
                }
                else if (args[0].Equals("-h") || args[0].Equals("--help"))
                {
                    PrintHelp();
                    return 0;
                }
                else
                {
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
                if (i == fileSystemInfo.Length - 1)
                {
                    PrintName(1, fileSystemInfo[i].Name);
                }
                else
                {
                    PrintName(0, fileSystemInfo[i].Name);
                }

                if (fileSystemInfo[i].Attributes.HasFlag(FileAttributes.Directory))
                {
                    if (i == fileSystemInfo.Length - 1)
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

        private static void PrintName(int flag, string name)
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

            string prefix = string.Empty;
            if (flag == 1)
            {
                prefix = "\\---";
            }
            else
            {
                prefix = "|--=";
            }

            string formatValue = string.Format("{0}{1} {2}", stringBuilder.ToString(), prefix, name);

            if (_isOutputRedirected)
            {
                Console.WriteLine(formatValue);
                return;
            }
            Console.WriteLine(formatValue.Length > _bufferWidth ? formatValue.Substring(0, _bufferWidth) : formatValue);
        }
    }
}
