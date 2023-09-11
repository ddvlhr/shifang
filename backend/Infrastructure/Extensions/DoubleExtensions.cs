using System;

namespace Infrastructure.Extensions
{
    public static class DoubleExtensions
    {
        // 格式化小数点
        public static string toString(this double value, int decimalCount)
        {
            var digital = "0.";
            for (var i = 0; i < decimalCount; i++)
            {
                digital += "0";
            }
            var result = Math.Round(value, decimalCount);
            return result.ToString(digital);
        }

        public static double format(this double value, int decimalCount)
        {
            return Math.Round(value, decimalCount);
        }

        public static double format(this double value)
        {
            var temp = value.ToString().Split(".");
            var count = 0;
            if (temp.Length == 2 && temp[1].Length > 0)
            {
                count = temp[1].Length;
            }
            return Math.Round(value, count);
        }
        
        public static int getDecimalCount(this double value)
        {
            var temp = value.ToString().Split(".");
            var count = 0;
            if (temp.Length == 2 && temp[1].Length > 0)
            {
                count = temp[1].Length;
            }
            return count;
        }

        public static string getDecimalCountStr(this double value)
        {
            var temp = value.ToString().Split(".");
            var count = 0;
            if (temp.Length == 2 && temp[1].Length > 0)
            {
                count = temp[1].Length;
            }

            var str = "0.";
            for (int i = 0; i < count; i++)
            {
                str += $"0";
            }

            if (count == 0)
                str = "#";

            return str;
        }
    }
}
