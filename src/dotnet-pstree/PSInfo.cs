using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet_pstree
{
    public class PSInfo
    {
        private static bool _isOutputRedirected = Console.IsOutputRedirected;
        private static int _bufferWidth = Console.BufferWidth;
        private static int _depth;
        private static int _line;

        internal int _index;
        private IList<PSInfo>? _children;

        public string User { get; set; } = string.Empty;
        public int PID { get; set; }
        public int PPID { get; set; }
        public int PGID { get; set; }
        public string Command { get; set; } = string.Empty;

        public IList<PSInfo> Children => _children ??= new List<PSInfo>();

        public void SetValue(string value)
        {
            switch (_index)
            {
                case 0:
                    User = value;
                    break;
                case 1:
                    PID = int.Parse(value);
                    break;
                case 2:
                    PPID = int.Parse(value);
                    break;
                case 3:
                    PGID = int.Parse(value);
                    break;
                case 4:
                    Command = value;
                    break;
                default:
                    throw new Exception();
            }
            ++_index;
        }

        public void PrintPSTree()
        {
            PrintPSInfo(0);
        }

        private void PrintPSInfo(int flag)
        {
            if (_depth > 10)
            {
                throw new Exception();
            }

            string prefix = string.Empty;
            if (flag == 0)
            {
                prefix = "-+=";
            }
            else if (Children.Count > 0 && flag == 4)
            {
                prefix = "|-+=";
            }
            else if (Children.Count > 0 && flag == 3)
            {
                prefix = "\\-+=";
            }
            else if (flag == 3)
            {
                prefix = "\\---";
            }
            else
            {
                prefix = "|--=";
            }

            PrintName(prefix);

            ++_depth;
            for (int i = 0; i < Children.Count; i++)
            {
                if (i == Children.Count - 1)
                {
                    Children[i].PrintPSInfo(3);
                }
                else
                {
                    _line ^= 1 << _depth;
                    Children[i].PrintPSInfo(4);
                    _line ^= 1 << _depth;
                }
            }
            --_depth;
        }

        private void PrintName(string prefix)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 1; i < _depth; i++)
            {
                int value = (_line >> i) & 1;
                if (value == 1)
                {
                    stringBuilder.Append("│ ");
                }
                else
                {
                    stringBuilder.Append("  ");
                }
            }

            string formatValue = string.Format(
                "{0}{1} {2} {3} {4}",
                stringBuilder.ToString(),
                prefix,
                PID,
                User,
                Command);

            if (_isOutputRedirected)
            {
                Console.WriteLine(formatValue);
                return;
            }
            Console.WriteLine(formatValue.Length > _bufferWidth ? formatValue.Substring(0, _bufferWidth) : formatValue);
        }
    }
}
