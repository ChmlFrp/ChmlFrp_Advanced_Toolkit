using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CAT.Views.Windows.Classes;

public static class Frp
{
    private static readonly Regex IniPathRegex =
        new(@"-c\s+([^\s]+\.ini)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static event Action OnFrpStatusChanged;

    public static async void StartFrp(string name, Button button = null)
    {
        var iniData = await GetFrpIniData(name);
        if (iniData == null)
        {
            OnFrpStatusChanged?.Invoke();
            return;
        }

        var frpciniFilePath = $"{Path.GetTempFileName()}.ini";
        var frpclogFilePath = Path.Combine(Paths.LogPath, $"{name}.logs");
        File.WriteAllText(frpciniFilePath, iniData);
        File.WriteAllText(frpclogFilePath, string.Empty);

        DispatcherTimer timer = null;
        if (button != null)
        {
            button.Content = "启动中";
            var dotCount = 0;
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            timer.Tick += (_, _) =>
            {
                dotCount = (dotCount + 1) % 4;
                button.Content = "启动中" + new string('.', dotCount);
            };
            timer.Start();
        }

        var frpProcess = new Process
        {
            StartInfo =
            {
                FileName = "cmd.exe",
                Arguments = $"/c {Paths.FrpExePath} -c {frpciniFilePath}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8
            }
        };

        frpProcess.OutputDataReceived += (_, args) =>
        {
            if (string.IsNullOrWhiteSpace(args.Data)) return;
            var logLine = args.Data.Replace(User.Usertoken, "{Usertoken}")
                .Replace(frpciniFilePath, "{IniFile}");
            const string pattern = @"^\d{4}/\d{2}/\d{2} \d{2}:\d{2}:\d{2} \[[A-Z]\] \[[^\]]+\] ?";
            logLine = Regex.Replace(logLine, pattern, "");
            File.AppendAllText(frpclogFilePath, logLine + Environment.NewLine, Encoding.UTF8);

            if (args.Data.Contains("启动成功"))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    OnFrpStatusChanged?.Invoke();
                    Reminders.Reminder_Box_Show($"{name}启动成功");
                });
                timer?.Stop();
            }
            else if (args.Data.Contains("[W]") || args.Data.Contains("[E]"))
            {
                var logContent = File.Exists(frpclogFilePath)
                    ? File.ReadAllText(frpclogFilePath)
                    : "日志文件不存在，请检查目录。";
                logContent = logContent.Contains("already exists")
                    ? $"{logContent}\n隧道已存在，无需再次启动。"
                    : $"{logContent}\n请把日志复制，千万别截图。。";
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CloseFrp(name);
                    OnFrpStatusChanged?.Invoke();
                    Reminders.Reminder_Box_Show($"{name}启动失败", "red");
                    Reminders.Reminder_Interface_Show($"{name}启动失败", logContent);
                });
                timer?.Stop();
            }
        };

        frpProcess.Start();
        frpProcess.BeginOutputReadLine();
    }

    public static async void CloseFrp(string name)
    {
        var frpcProcesses = Process.GetProcessesByName("frpc");
        if (frpcProcesses.Length == 0)
        {
            Reminders.Reminder_Box_Show($"{name}关闭失败", "red");
            OnFrpStatusChanged?.Invoke();
            return;
        }

        await Task.Run(() =>
        {
            foreach (var frpcProcess in frpcProcesses.Where(frpcProcess => frpcProcess != null))
                KillProcess(frpcProcess);
        });

        Reminders.Reminder_Box_Show($"{name}关闭成功");
        OnFrpStatusChanged?.Invoke();
    }

    public static bool FrpExists(string name)
    {
        var processes = Process.GetProcessesByName("frpc");
        var iniCache = new ConcurrentDictionary<string, List<string>>();
        var parser = new FileIniDataParser();

        var exists = processes.AsParallel().Any(process =>
        {
            var commandLine = GetCommandLine(process);
            if (string.IsNullOrWhiteSpace(commandLine)) return false;

            var frpcinipath = GetIniPath(process, commandLine);
            if (string.IsNullOrWhiteSpace(frpcinipath) || !File.Exists(frpcinipath)) return false;

            var tunnelNames = iniCache.GetOrAdd(frpcinipath, path =>
            {
                try
                {
                    var data = parser.ReadFile(path);
                    return data.Sections
                        .Where(section => section.SectionName != "common")
                        .Select(section => section.SectionName)
                        .ToList();
                }
                catch
                {
                    return [];
                }
            });

            return tunnelNames.Contains(name);
        });

        return exists;
    }

    private static async Task<string> GetFrpIniData(string name)
    {
        var parameters = new Dictionary<string, string> { { "token", $"{User.Usertoken}" } };
        var jObject = await Api.GetApi("https://cf-v2.uapis.cn/tunnel", parameters);

        if (jObject == null)
        {
            Reminders.Reminder_Box_Show("获取隧道列表失败", "red");
            return null;
        }

        var node = (from tunnel in jObject["data"]!
            where tunnel["name"]?.ToString() == name
            select tunnel["node"]?.ToString()).FirstOrDefault();

        if (node == null)
        {
            Reminders.Reminder_Box_Show("获取隧道节点失败", "red");
            return null;
        }

        parameters = new Dictionary<string, string>
        {
            { "token", $"{User.Usertoken}" },
            { "node", $"{node}" },
            { "tunnel_names", $"{name}" }
        };
        jObject = await Api.GetApi("https://cf-v2.uapis.cn/tunnel_config", parameters);

        if (jObject != null) return jObject["data"]?.ToString();

        Reminders.Reminder_Box_Show("获取隧道配置失败", "red");
        return null;
    }

    private static string GetIniPath(Process process, string commandLine)
    {
        if (commandLine == "frpc")
            return Path.Combine(Path.GetDirectoryName(process.MainModule?.FileName ?? "") ?? "", "frpc.ini");

        var match = IniPathRegex.Match(commandLine);
        return match.Success ? match.Groups[1].Value : null;
    }

    private static string GetCommandLine(Process process)
    {
        try
        {
            using var searcher = new ManagementObjectSearcher(
                $"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {process.Id}");
            return searcher.Get()
                .Cast<ManagementObject>()
                .Select(obj => obj["CommandLine"]?.ToString())
                .FirstOrDefault();
        }
        catch
        {
            return null;
        }
    }

    private static void KillProcess(Process process)
    {
        if (process == null) return;

        Process.Start(new ProcessStartInfo
        {
            FileName = "taskkill",
            Arguments = $"/PID {process.Id} /T /F",
            UseShellExecute = false,
            CreateNoWindow = true
        })?.WaitForExit();
    }
}