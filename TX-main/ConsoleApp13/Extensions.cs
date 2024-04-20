using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp13
{
    internal static class Extensions
    {
        public static string ClearSubstringIndexRange( string input, int startIndex, int endIndex)
        {
            if (string.IsNullOrEmpty(input) || startIndex < 0 || endIndex < 0 || startIndex > endIndex)
            {
                throw new ArgumentException("Invalid input or index values.");
            }

            if (startIndex >= input.Length)
            {
                return input;
            }

            if (endIndex >= input.Length)
            {
                endIndex = input.Length - 1;
            }

            var result = new StringBuilder(input.Length);
            for (int i = 0; i < input.Length; i++)
            {
                if (i < startIndex || i > endIndex)
                {
                    result.Append(input[i]);
                }
                else
                {
                    result.Append(' ');
                }
            }

            return result.ToString().Trim();
        }


    }


}
