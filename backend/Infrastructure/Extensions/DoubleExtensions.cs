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
    }
}
