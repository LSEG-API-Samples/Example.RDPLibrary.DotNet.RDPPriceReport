using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChainExpanderLib.Utils
{
    internal class ChainComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            try
            {
                var tempX = x.Split('#').ToList();
                var tempY = y.Split('#').ToList();
                var headRicX = tempX.Count > 1 ? tempX[0].Replace('T', ' ').Trim() : "0";
                var headRicY = tempY.Count > 1 ? tempY[0].Replace('T', ' ').Trim() : "0";
                if (string.IsNullOrEmpty(headRicX)) headRicX = "0";
                if (string.IsNullOrEmpty(headRicY)) headRicY = "0";
                var numX = Convert.ToInt32(headRicX, IsHexString(headRicX) ? 16 : 10);
                var numY = Convert.ToInt32(headRicY, IsHexString(headRicY) ? 16 : 10);
                if (numX > numY)
                    return 1;
                else if (numY > numX)
                    return -1;
                return string.Compare(x, y, StringComparison.Ordinal);
            }
            catch
            {

                return -1;
            }

        }
        public static bool IsHexString(string inputstring)
        {
            // For C-style hex notation (0xFF) you can use @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z"
            return System.Text.RegularExpressions.Regex.IsMatch(inputstring, @"\A\b[0-9a-fA-F]+\b\Z");
        }
    };
}
