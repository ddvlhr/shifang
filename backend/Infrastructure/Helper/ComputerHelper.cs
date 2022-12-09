using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace Infrastructure.Helper;

public class ComputerHelper
{
    /// <summary>
    /// 获取计算机信息
    /// </summary>
    /// <returns></returns>
    public static MemoryMetrics GetComputerInfo()
    {
        try
        {
            MemoryMetricsClient client = new();
            var memoryMetrics = IsUnix() ? MemoryMetricsClient.GetUnixMetrics() : MemoryMetricsClient.GetWindowsMetrics();

            memoryMetrics.FreeRam = Math.Round(memoryMetrics.Free / 1024, 2) + " GB";
            memoryMetrics.UsedRam = Math.Round(memoryMetrics.Used / 1024, 2) + " GB";
            memoryMetrics.TotalRam = Math.Round(memoryMetrics.Total / 1024, 2) + " GB";
            memoryMetrics.RamRate = Math.Ceiling(100 * memoryMetrics.Used / memoryMetrics.Total);
            memoryMetrics.RamUsage = memoryMetrics.RamRate + " %";
            memoryMetrics.CpuRate = Math.Ceiling(double.Parse(GetCpuRate()));
            memoryMetrics.CpuUsage = memoryMetrics.CpuRate + " %";
            return memoryMetrics;
        }
        catch (Exception ex)
        {
            Console.WriteLine("获取内存使用出错, msg=" + ex.Message + "," + ex.StackTrace);
        }

        return new MemoryMetrics();
    }
    
    /// <summary>
    /// 判断是否是Unix类系统(linux/OSX)
    /// </summary>
    /// <returns></returns>
    public static bool IsUnix()
    {
        var isUnix = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                     RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        return isUnix;
    }

    public static string GetCpuRate()
    {
        string cpuRate;
        if (IsUnix())
        {
            var output = ShellHelper.Bash("top -b n1 | grep \"Cpu(s)\" | awk '{print $2 + $4}'");
            cpuRate = output.Trim();
        }
        else
        {
            var output = ShellHelper.Cmd("wmic", "cpu get LoadPercentage");
            cpuRate = output.Replace("LoadPercentage", string.Empty).Trim();
        }

        return cpuRate;
    }
    public class MemoryMetrics
    {
        [JsonIgnore]
        public double Total { get; set; }
        [JsonIgnore]
        public double Used { get; set; }
        [JsonIgnore]
        public double Free { get; set; }
        public string UsedRam { get; set; }
        public double CpuRate { get; set; }
        public string CpuUsage { get; set; }
        public string TotalRam { get; set; }
        public double RamRate { get; set; }
        public string RamUsage { get; set; }
        public string FreeRam { get; set; }
    }

    public class MemoryMetricsClient
    {
        #region 获取内存信息

        /// <summary>
        /// windows 系统获取
        /// </summary>
        /// <returns></returns>
        public static MemoryMetrics GetWindowsMetrics()
        {
            var output = ShellHelper.Cmd("wmic", "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value");
            var metrics = new MemoryMetrics();
            var lines = output.Trim().Split("\r\r\n", StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length <= 0) return metrics;

            var freeMemoryParts = lines[0].Split('=', StringSplitOptions.RemoveEmptyEntries);
            var totalMemoryParts = lines[1].Split('=', StringSplitOptions.RemoveEmptyEntries);

            metrics.Total = Math.Round(double.Parse(totalMemoryParts[1]) / 1024, 0);
            metrics.Free = Math.Round(double.Parse(freeMemoryParts[1]) / 1024, 0);
            metrics.Used = metrics.Total - metrics.Free;

            return metrics;
        }

        /// <summary>
        /// Unix 系统获取
        /// </summary>
        /// <returns></returns>
        public static MemoryMetrics GetUnixMetrics()
        {
            var output = ShellHelper.Bash("free -m | awk '{print $2,$3,$4,$5,$6}'");
            var metrics = new MemoryMetrics();
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length <= 0) return metrics;

            if (lines.Length > 0)
            {
                var memory = lines[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (memory.Length >= 3)
                {
                    metrics.Total = double.Parse(memory[0]);
                    metrics.Used = double.Parse(memory[1]);
                    metrics.Free = double.Parse(memory[2]);
                }
            }

            return metrics;
        }

        #endregion
    }
}