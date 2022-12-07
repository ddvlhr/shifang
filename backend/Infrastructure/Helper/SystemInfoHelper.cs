using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Core.Models;

namespace Infrastructure.Helper;

public static class SystemInfoHelper
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MEMORY_INFO
    {
        // 当前结构体大小
        public uint dwLength;
        // 当前内存使用率
        public uint dwMemoryLoad;
        // 总计物理内存大小
        public long ullTotalPhys;
        // 可用物理内存大小
        public long ullAvailPhys;
        // 总计交换文件大小
        public long ullTotalPageFile;
        // 可用交换文件大小
        public long ullAvailPageFile;
        // 总计虚拟内存大小
        public long ullTotalVirtual;
        // 可用虚拟内存大小
        public long ullAvailVirtual;
        // 保留 这个值始终为0
        public long ullAvailExtendedVirtual;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx(ref MEMORY_INFO lpBuffer);

    private static Tuple<double, string> FormatSize(double size, int sizeType = 5)
    {
        var d = (double)size;
        var i = 0;
        while ((d > 1024) && (i < 5))
        {
            d /= 1024;
            i++;
        }
        var unit = new[] { "B", "KB", "MB", "GB", "TB" };
        var desc = string.Format("{0} {1}", Math.Round(d, 2), unit[i]);
        var result = new Tuple<double, string>(Math.Round(d, 2), desc);
        return result;
    }

    public static MEMORY_INFO GetMemoryStatus()
    {
        var memoryInfo = new MEMORY_INFO();
        memoryInfo.dwLength = (uint)Marshal.SizeOf(memoryInfo);
        GlobalMemoryStatusEx(ref memoryInfo);
        return memoryInfo;
    }

    /// <summary>
    /// 获取当前可用物理内存大小
    /// </summary>
    /// <returns>当前可用物理内存(B)</returns>
    public static ulong GetAvailPhys()
    {
        var memory = GetMemoryStatus();
        return (ulong)memory.ullAvailPhys;
    }

    /// <summary>
    /// 获取当前已用物理内存大小
    /// </summary>
    /// <returns>当前已用物理内存(B)</returns>
    public static ulong GetUsedPhys()
    {
        var memory = GetMemoryStatus();
        return (ulong)(memory.ullTotalPhys - memory.ullAvailPhys);
    }

    /// <summary>
    /// 获取当前总物理内存大小
    /// </summary>
    /// <returns>当前总物理内存(B)</returns>
    public static ulong GetTotalPhys()
    {
        var memory = GetMemoryStatus();
        return (ulong)memory.ullTotalPhys;
    }
    public static async Task<SystemInfo> GetSystemInfo()
    {
        var systemInfo = new SystemInfo()
        {
            Version = Environment.OSVersion.VersionString,
        };

        var cpuInfo = Masuit.Tools.Hardware.CpuInfo.Locals;
        // var cpuLoad = Masuit.Tools.Hardware.SystemInfo.GetProcessorData();
        // systemInfo.Cpu = cpuLoad;
        systemInfo.CpuCounter = GetCpuLoad();
        var allRam = GetTotalPhys();
        var usedRam = GetUsedPhys();
        var ramUseage = Math.Round((((double)usedRam / allRam) * 100), 2);
        systemInfo.UsedRam = FormatSize(GetUsedPhys()).Item2;
        systemInfo.AllRam = FormatSize(allRam).Item2;
        systemInfo.RamUseage = ramUseage;

        return systemInfo;
    }

    public static double GetCpuLoad()
    {
        using (var p = new Process())
        {
            p.StartInfo.FileName = "wmic.exe";
            p.StartInfo.Arguments = "cpu get loadpercentage";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            int load = -1;
            var m = System.Text.RegularExpressions.Regex.Match(
                p.StandardOutput.ReadToEnd(), @"\d+");
            if (m.Success) load = int.Parse(m.Value);
            p.WaitForExit();
            return load;
        }
    }

    public static async Task<double> GetCpuUseage()
    {
        var startTime = DateTime.UtcNow;
        var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

        await Task.Delay(500);

        var endTime = DateTime.UtcNow;
        var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

        var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
        var totalMsPassed = (endTime - startTime).TotalMilliseconds;

        var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

        return cpuUsageTotal * 100;
    }
}