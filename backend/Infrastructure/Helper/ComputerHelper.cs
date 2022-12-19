using System;
using System.Runtime.InteropServices;
using CZGL.SystemInfo;
using Newtonsoft.Json;

namespace Infrastructure.Helper;

public class ComputerHelper
{
    /// <summary>
    /// 获取计算机信息
    /// </summary>
    /// <returns></returns>
    public static ComputerInfo GetComputerInfo()
    {
        try
        {
            MemoryMetricsClient client = new();
            var memoryMetrics = IsUnix() ? MemoryMetricsClient.GetUnixMetrics() : MemoryMetricsClient.GetWindowsMetrics();
            var cpu = new CZGL.SystemInfo.CPUTime();
            memoryMetrics.FrameworkDescription = SystemPlatformInfo.FrameworkDescription;
            memoryMetrics.SystemDescription = SystemPlatformInfo.OSVersion;
            memoryMetrics.SystemRuntimes = cpu.SystemTime.ToString();
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

        return new ComputerInfo();
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
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var output = ShellHelper.Bash("top -b n1 | grep \"Cpu(s)\" | awk '{print $2 + $4}'");
            cpuRate = output.Trim();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var output = ShellHelper.Bash("top -l 1 | head -n 10 | grep \"CPU usage\" | awk '{print $3}' | cut -d \"%\" -f 1");
            cpuRate = output.Trim();
        }
        else
        {
            var output = ShellHelper.Cmd("wmic", "cpu get LoadPercentage");
            cpuRate = output.Replace("LoadPercentage", string.Empty).Trim();
        }

        return cpuRate;
    }
    public class ComputerInfo
    {
        [JsonIgnore]
        public double Total { get; set; }
        [JsonIgnore]
        public double Used { get; set; }
        [JsonIgnore]
        public double Free { get; set; }

        public string FrameworkDescription { get; set; }

        public string SystemDescription { get; set; }
        public string SystemRuntimes { get; set; }
        public string UsedRam { get; set; }
        public double CpuRate { get; set; } = 0;
        public string CpuUsage { get; set; }
        public string TotalRam { get; set; }
        public double RamRate { get; set; } = 0;
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
        public static ComputerInfo GetWindowsMetrics()
        {
            var output = ShellHelper.Cmd("wmic", "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value");
            var metrics = new ComputerInfo();
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
        public static ComputerInfo GetUnixMetrics()
        {
            var metrics = new ComputerInfo();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var output = ShellHelper.Bash("free -m | awk '{print $2,$3,$4,$5,$6}'");
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
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var output = ShellHelper.Bash("top -l 1 | head -n 10 | grep PhysMem | awk '{print $2, $8}'");
                var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                if (lines.Length <= 0) return metrics;

                if (lines.Length > 0)
                {
                    var memory = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (memory.Length >= 2)
                    {
                        double used = 0;
                        double free = 0;
                        var usedStr = memory[0];
                        if (usedStr.Contains('G'))
                        {
                            used = double.Parse(usedStr.Replace("G", "")) * 1024;
                        }
                        else
                        {
                            used = double.Parse(usedStr.Replace("M", ""));
                        }
                        var freeStr = memory[1];
                        if (freeStr.Contains('G'))
                        {
                            free = double.Parse(freeStr.Replace("G", "")) * 1024;
                        }
                        else
                        {
                            free = double.Parse(freeStr.Replace("M", ""));
                        }

                        metrics.Used = used;
                        metrics.Free = free;
                        metrics.Total = 16 * 1024;
                    }
                }
            }

            return metrics;
        }

        #endregion
    }
}