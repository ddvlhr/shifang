using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Extensions;

public static class StatisticExtensions
{
    public static StatisticItem toStatistic(this IEnumerable<double> list, string standard, string upper,
        string lower, bool equal = true)
    {
        var result = new StatisticItem();
        if (string.IsNullOrEmpty(standard))
            return result;
        result = list.toStatistic(double.Parse(standard), double.Parse(upper), double.Parse(lower));
        return result;
    }

    public static StatisticItem toStatistic(this IEnumerable<double> list, double standard, double upper, double
        lower, bool equal = true)
    {
        var result = new StatisticItem();
        var data = list.ToList();
        if (!data.Any())
            return result;

        var length = data.Count;
        var high = standard + upper;
        var low = standard - lower;
        var moreThan = false;
        var lessThan = false;
        if (standard.Equals(upper))
        {
            high = standard;
            lessThan = true;
        }
        else
        {
            lessThan = false;
        }

        if (standard.Equals(lower))
        {
            low = standard;
            moreThan = true;
        }
        else
        {
            moreThan = false;
        }

        result.Max = data.Max();
        result.Min = data.Min();
        result.Mean = data.Average();

        foreach (var d in data)
        {
            result.Sd += Math.Pow(Math.Abs(d - result.Mean), 2);
            if (moreThan)
            {
                if (d < low)
                    ++result.LowCnt;
            }
            else if (lessThan)
            {
                if (d > high)
                    ++result.HighCnt;
            }
            else
            {
                if (d > high)
                    ++result.HighCnt;
                else if (d < low)
                    ++result.LowCnt;
            }

            result.Offset += Math.Abs(d - standard);
        }

        var quality = length - result.HighCnt - result.LowCnt;
        result.Quality = $"{quality} / {length}";
        var rate = Convert.ToDouble(quality) / length * 100;
        result.QualityRate = $"{rate:F2} %";

        result.Offset /= length;

        if (length <= 1)
        {
            result.Sd = 0;
            return result;
        }

        result.Sd /= length - 1;
        result.Sd = Math.Sqrt(result.Sd);
        result.Cv = result.Mean.Equals(0) ? 0 : result.Sd / result.Mean;
        result.Cv *= 100;
        result.MeanOffset = result.Mean - standard;

        /* 标准差修正 */
        if (result.Sd < 0.00001)
        {
            result.Sd = 0;
            return result;
        }

        if (low.Equals(0)) // 圆度，没有下限
            result.Cpk = Math.Abs(high - result.Mean) / (3 * result.Sd);
        else
            result.Cpk = Math.Min(Math.Abs(high - result.Mean), Math.Abs(result.Mean - low)) / (3 * result.Sd);

        return result;
    }
}

public class StatisticItem
{
    public double Mean { get; set; }
    public double MeanOffset { get; set; }
    public double Max { get; set; }
    public double Min { get; set; }
    public double Sd { get; set; }
    public double Cv { get; set; }
    public double Cpk { get; set; }
    public double Offset { get; set; }
    public int HighCnt { get; set; }
    public int LowCnt { get; set; }
    public string Quality { get; set; }
    public string QualityRate { get; set; }
}