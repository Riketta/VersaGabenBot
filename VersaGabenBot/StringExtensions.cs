using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace VersaGabenBot
{
    internal static class StringExtensions
    {
        public static string[] SplitByLengthAtNewLine(this string str, int length)
        {
            List<string> result = new List<string>();
            StringBuilder sb = new StringBuilder(length);
            int currentLength = 0;

            foreach (string line in str.Split('\n'))
            {
                if (currentLength + line.Length > length)
                {
                    result.Add(sb.ToString());
                    sb.Clear();
                    currentLength = 0;
                }

                currentLength += line.Length;
                sb.Append(line).Append('\n');
            }
            result.Add(sb.ToString());

            return result.ToArray();
        }
    }
}
