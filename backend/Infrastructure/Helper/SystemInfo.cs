using System;
using System.Diagnostics;
using System.Text;

namespace Infrastructure.Helper;

public static class SystemInfo
{
    public static string GetSystemInfo()
    {
        var sb = new StringBuilder();
        sb.AppendLine("OS Version: " + Environment.OSVersion);
        sb.AppendLine("OS Version (Service Pack): " + Environment.OSVersion.ServicePack);
        sb.AppendLine("OS Version (Version String): " + Environment.OSVersion.VersionString);
        sb.AppendLine("OS Version (Version): " + Environment.OSVersion.Version);
        sb.AppendLine("OS Version (Major): " + Environment.OSVersion.Version.Major);
        sb.AppendLine("OS Version (Minor): " + Environment.OSVersion.Version.Minor);
        sb.AppendLine("OS Version (Build): " + Environment.OSVersion.Version.Build);
        sb.AppendLine("OS Version (Revision): " + Environment.OSVersion.Version.Revision);
        sb.AppendLine("OS Version (Major Revision): " + Environment.OSVersion.Version.MajorRevision);
        sb.AppendLine("OS Version (Minor Revision): " + Environment.OSVersion.Version.MinorRevision);
        sb.AppendLine("OS Version (Platform): " + Environment.OSVersion.Platform);
        sb.AppendLine("OS Version (Service Pack): " + Environment.OSVersion.ServicePack);
        sb.AppendLine("OS Version (Version String): " + Environment.OSVersion.VersionString);
        sb.AppendLine("OS Version (Version): " + Environment.OSVersion.Version);
        sb.AppendLine("OS Version (Major): " + Environment.OSVersion.Version.Major);
        sb.AppendLine("OS Version (Minor): " + Environment.OSVersion.Version.Minor);
        sb.AppendLine("OS Version (Build): " + Environment.OSVersion.Version.Build);
        sb.AppendLine("OS Version (Revision): " + Environment.OSVersion.Version.Revision);
        sb.AppendLine("OS Version (Major Revision): " + Environment.OSVersion.Version.MajorRevision);
        sb.AppendLine("OS Version (Minor Revision): " + Environment.OSVersion.Version.MinorRevision);
        sb.AppendLine("OS Version (Platform): " + Environment.OSVersion.Platform);
        sb.AppendLine("OS Version (Service Pack): " + Environment.OSVersion.ServicePack);
        sb.AppendLine("OS Version (Version String): " + Environment.OSVersion.VersionString);
        sb.AppendLine("OS Version (Version): " + Environment.OSVersion.Version);
        sb.AppendLine("OS Version (Major): " + Environment.OSVersion.Version.Major);
        sb.AppendLine("OS Version (Minor): " + Environment.OSVersion.Version.Minor);
        sb.AppendLine("OS Version (Build): " + Environment.OSVersion.Version.Build);
        sb.AppendLine("OS Version (Revision): " + Environment.Version);

        return sb.ToString();
    }

    public static string GetMemoryInfo()
    {
        Process[] ps = Process.GetProcesses();
        long totalMemory = 0;
        var info = "";
        foreach (Process p in ps)
        {
            totalMemory += p.WorkingSet64 / 1024;
            info += p.ProcessName + "内存占用：" + p.WorkingSet64 / 1024 + "KB" + Environment.NewLine;
        }
        
        var result = $"系统总内存: { totalMemory / 1024 } MB";
        result += info;
        return result;
    }
}