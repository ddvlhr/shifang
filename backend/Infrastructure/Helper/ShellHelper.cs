﻿using System.Diagnostics;

namespace Infrastructure.Helper;

public class ShellHelper
{
    /// <summary>
    /// linux 系统命令
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static string Bash(string command)
    {
        var escapedArgs = command.Replace("\"", "\\\"");
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{escapedArgs}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        var result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        process.Dispose();
        return result;
    }

    /// <summary>
    /// windows 系统命令
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string Cmd(string fileName, string args)
    {
        var output = string.Empty;

        var info = new ProcessStartInfo()
        {
            FileName = fileName,
            Arguments = args,
            RedirectStandardOutput = true
        };

        using var process = Process.Start(info);
        if (process != null) output = process.StandardOutput.ReadToEnd();
        return output;
    }
}