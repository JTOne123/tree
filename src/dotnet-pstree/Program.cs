using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace dotnet_pstree
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.WriteLine("Windows not supported.");
                return 1;
            }

            if (args.Length > 0)
            {
                if (args[0].Equals("-v") || args[0].Equals("--version"))
                {
                    Console.WriteLine("0.2");
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

            using StreamReader standardOutput = RunPSCommand();

            IList<PSInfo> psInfos = GetPSInfos(standardOutput);
            PSInfo psInfo = FindRootPSInfo(psInfos);
            psInfo.PrintPSTree();

            return 0;
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Usage: dotnet-pstree [options]");
            Console.WriteLine("Options");
            Console.WriteLine("  -v --version                      Print version.");
            Console.WriteLine("  -h --help                         Print usage.");
        }

        private static PSInfo FindRootPSInfo(IList<PSInfo> psInfos)
        {
            PSInfo? rootPSInfo = null;
            for (int i = 0; i < psInfos.Count; i++)
            {
                if (psInfos[i].PPID == 0)
                {
                    rootPSInfo = psInfos[i];
                    continue;
                }

                for (int j = 0; j < psInfos.Count; j++)
                {
                    if (psInfos[i].PPID == psInfos[j].PID)
                    {
                        psInfos[j].Children.Add(psInfos[i]);
                        break;
                    }
                }
            }

            if (rootPSInfo == null)
            {
                throw new Exception();
            }
            return rootPSInfo;
        }

        private static StreamReader RunPSCommand()
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "ps";
            processStartInfo.Arguments = "-axwwo user,pid,ppid,pgid,command";
            processStartInfo.RedirectStandardOutput = true;

            Process process = Process.Start(processStartInfo);
            process.WaitForExit();

            return process.StandardOutput;
        }

        private static string[] GetPSHeaders(string header)
        {
            return header.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        private static IList<PSInfo> GetPSInfos(StreamReader standardOutput)
        {
            List<PSInfo> psInfos = new List<PSInfo>();

            string? header = standardOutput.ReadLine();
            if (header == null)
            {
                return psInfos;
            }

            for (; ; )
            {
                string? line = standardOutput.ReadLine();
                if (line == null)
                {
                    break;
                }

                PSInfo psInfo = new PSInfo();

                int index = 0;
                int flag = 0;
                for (int i = 0; i < line.Length; i++)
                {
                    if (flag == 0 && line[i] != ' ')
                    {
                        index = i;
                        flag = 1;
                    }
                    else if (flag == 1 && psInfo._index == 4)
                    {
                        flag = 0;
                        psInfo.SetValue(line.Substring(index));
                        break;
                    }
                    else if (flag == 1 && line[i] == ' ')
                    {
                        flag = 0;
                        psInfo.SetValue(line.Substring(index, i - index));
                    }
                    else if (flag == 1 && i == line.Length - 1)
                    {
                        flag = 0;
                        psInfo.SetValue(line.Substring(index, i - index));
                    }
                }

                if (flag == 1)
                {
                    throw new Exception();
                }

                psInfos.Add(psInfo);
            }
            return psInfos;
        }
    }
}
