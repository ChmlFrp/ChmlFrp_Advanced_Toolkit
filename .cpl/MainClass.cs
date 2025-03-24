using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ChmlFrp_Professional_Launcher.Pages;
using IniParser;
using IniParser.Model;
using Newtonsoft.Json.Linq;

namespace ChmlFrp_Professional_Launcher
{
    internal class MainClass
    {
        public static MainWindow MainWindowClass;

        internal static class PagesClass
        {
            public static ChmlFrphomePage ChmlFrpHomePage = new();
            public static LaunchPage LaunchPage = new();
        }

        public static bool IsProcess(string name)
        {
            if (Process.GetProcessesByName(name).Length > 1)
                return true;
            return false;
        }

        public static volatile bool SignInBool;

        //初始化
        public static void Initialize()
        {
            new Paths();

            // 检测是否有两个ChmlFrp Professional Launcher进程
            if (IsProcess(Process.GetCurrentProcess().ProcessName))
            {
                MainWindowClass.Close();
                return;
            }

            try
            {
                //检测是否有相关配置文件
                if (!File.Exists(Paths.CPLPath))
                {
                    Directory.CreateDirectory(Paths.CPLPath);
                }
                if (!File.Exists(Paths.pictures_path))
                {
                    Directory.CreateDirectory(Paths.pictures_path);
                }
                if (File.Exists(Path.Combine(Paths.CPLPath, "update.bat")))
                {
                    File.Delete(Path.Combine(Paths.CPLPath, "update.bat"));
                }
                if (!File.Exists(Paths.IniPath))
                {
                    Directory.CreateDirectory(Paths.IniPath);
                }
                if (!File.Exists(Paths.setupIniPath))
                {
                    IniData data = new();
                    FileIniDataParser parser = new();
                    data.Sections.AddSection("ChmlFrp_Professional_Launcher Setup");
                    parser.WriteFile(Paths.setupIniPath, data);
                }
            }
            catch
            {
                Reminders.LogsOutputting("文件占用无法创建");
            }
        }

        internal class Paths
        {
            //定义路径
            public static string directoryPath = Directory.GetCurrentDirectory();

            public static string IniPath;
            public static string frpExePath;
            public static string setupIniPath;
            public static string CPLPath;
            public static string pictures_path;
            public static string logfilePath;

            public Paths()
            {
                CPLPath = Path.Combine(directoryPath, "CPL");
                pictures_path = Path.Combine(CPLPath, "Pictures");
                IniPath = Path.Combine(CPLPath, "Ini");
                logfilePath = Path.Combine(CPLPath, "Debug.logs");
                setupIniPath = Path.Combine(CPLPath, "Setup.ini");
                frpExePath = Path.Combine(CPLPath, "frpc.exe");
                new Temp();
            }

            internal class Temp
            {
                public static string temp_api_tunnel;
                public static string temp_api_update;
                public static string temp_api_login;
                public static string temp_api_user;
                public static string temp_user_image;

                public Temp()
                {
                    temp_api_tunnel = Path.GetTempFileName();
                    temp_api_update = Path.GetTempFileName();
                    temp_api_login = Path.GetTempFileName();
                    temp_api_user = Path.GetTempFileName();
                    temp_user_image = Path.GetTempFileName();
                }
            }
        }

        internal class User
        {
            public static string username;
            public static string password;
            public static string usertoken;

            public User()
            {
                var data = new FileIniDataParser().ReadFile(Paths.setupIniPath);
                username = data["ChmlFrp_Professional_Launcher Setup"]["Username"];
                password = data["ChmlFrp_Professional_Launcher Setup"]["Password"];
                usertoken = data["ChmlFrp_Professional_Launcher Setup"]["Token"];
                //string jsonContent = File.ReadAllText(Paths.temp_api_path);
                //var jsonObject = JObject.Parse(jsonContent);
            }
        }

        internal static class Downloadfiles
        {
            public static bool Download(string url, string path)
            {
                if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(path))
                {
                    Reminders.LogsOutputting("下载失败：NullOrWhiteSpace");
                    return false;
                }

                try
                {
                    using WebClient client = new();
                    client.Encoding = Encoding.UTF8;
                    client.DownloadFile(new Uri(url), path);
                }
                catch
                {
                    Reminders.LogsOutputting($"下载失败：文件占用或网络错误?path={path}&url={url}");
                    return false;
                }
                Reminders.LogsOutputting($"下载成功：已下载?path={path}");
                return true;
            }

            public static async Task<bool> Downloadasync(string url, string path)
            {
                if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(path))
                {
                    Reminders.LogsOutputting("下载失败：NullOrWhiteSpace");
                    return false;
                }

                try
                {
                    using WebClient client = new();
                    client.Encoding = Encoding.UTF8;
                    await client.DownloadFileTaskAsync(new Uri(url), path);
                }
                catch
                {
                    Reminders.LogsOutputting($"下载失败：文件占用或网络错误?path={path}&url={url}");
                    return false;
                }
                Reminders.LogsOutputting($"下载成功：已下载?path={path}");
                return true;
            }

            public static bool GetAPItoLogin(bool Remind)
            {
                new User();

                if (
                    Download(
                        $"https://cf-v2.uapis.cn/login?username={User.username}&password={User.password}",
                        Paths.Temp.temp_api_login
                    )
                )
                {
                    string msg = JObject
                        .Parse(File.ReadAllText(Paths.Temp.temp_api_login))["msg"]
                        ?.ToString();
                    Reminders.LogsOutputting("API提醒：" + msg);
                    if (msg == "登录成功")
                    {
                        SignInBool = true;
                        if (Remind)
                            Reminders.Reminder_Box_Show(msg);
                        return true;
                    }
                    else
                    {
                        if (Remind)
                            Reminders.Reminder_Box_Show(msg, "red");
                    }
                }
                else
                {
                    if (Remind)
                        Reminders.Reminder_Box_Show("网络错误", "red");
                }
                SignInBool = false;
                return false;
            }
        }

        internal static class Reminders
        {
            private static int i = 1;
            private static Reminder_Box_Show Reminder_Box_Show_Window = new();
            private static Reminder_Interface_Show Reminder_Interface_Show_Window = new();
            public static Reminder_Download_Show Reminder_Download_Show_Window = new();

            public static void LogsOutputting(string logEntry)
            {
                switch (i)
                {
                    case 1:
                        //清空文件
                        File.WriteAllText(Paths.logfilePath, string.Empty);
                        i++;
                        break;
                }
                logEntry = $"[{DateTime.Now}] " + logEntry;
                Console.WriteLine(logEntry);
                File.AppendAllText(Paths.logfilePath, logEntry + Environment.NewLine);
            }

            public static void Reminder_Box_Show(string message, string color = "green")
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    LogsOutputting("提醒消息不能为空或全为空格");
                    return;
                }

                if (Reminder_Box_Show_Window.Visibility == Visibility.Collapsed)
                {
                    Reminder_Box_Show_Window.Visibility = Visibility.Visible;
                }

                if (color == "green")
                {
                    Reminder_Box_Show_Window.RemindersBorder.Background = new SolidColorBrush(
                        Colors.LimeGreen
                    );
                }
                else if (color == "blue")
                {
                    Reminder_Box_Show_Window.RemindersBorder.Background = new SolidColorBrush(
                        Colors.DodgerBlue
                    );
                }
                else if (color == "red")
                {
                    Reminder_Box_Show_Window.RemindersBorder.Background = new SolidColorBrush(
                        Colors.Red
                    );
                }
                else if (color == "yellow")
                {
                    Reminder_Box_Show_Window.RemidingTextBlock.Foreground = new SolidColorBrush(
                        Colors.Green
                    );
                    Reminder_Box_Show_Window.RemindersBorder.Background = new SolidColorBrush(
                        Colors.Yellow
                    );
                }
                Reminder_Box_Show_Window.RemidingTextBlock.Text = message;
                MainWindowClass.RemindersNavigationTwo.Navigate(Reminder_Box_Show_Window);
            }

            public static void Reminder_Interface_Show(
                string subject,
                string message,
                bool isUpdate = false,
                string url = ""
            )
            {
                if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(message))
                {
                    LogsOutputting("提醒消息不能为空或全为空格");
                    return;
                }

                if (Reminder_Interface_Show_Window.Visibility == Visibility.Collapsed)
                {
                    Reminder_Interface_Show_Window.Visibility = Visibility.Visible;
                }

                Reminder_Interface_Show_Window.Yes_CornerButten.IsSelected = true;

                if (isUpdate)
                {
                    Reminder_Interface_Show_Window.Yes_CornerButten.Content = "更新";
                    Reminder_Interface_Show_Window.Yes_CornerButten.Click += async (sender, e) =>
                    {
                        Reminder_Interface_Show_Window.Visibility = Visibility.Collapsed;

                        await Task.Delay(1000);

                        string EXE = Path.Combine(
                            Paths.CPLPath,
                            "ChmlFrp_Professional_Launcher.exe"
                        );

                        if (Downloadfiles.Download(url, EXE))
                        {
                            Reminders.Reminder_Box_Show("下载成功");
                            Reminders.LogsOutputting("下载成功");

                            string batchFilePath = Path.Combine(Paths.CPLPath, "update.bat");

                            // 创建批处理文件内容
                            string batchContent =
                                $@"
        @echo off
        timeout /t 3 /nobreak
        move /y ""{EXE}"" ""{Process.GetCurrentProcess().MainModule.FileName}""
        start """" ""{Process.GetCurrentProcess().MainModule.FileName}""
        exit
        ";

                            // 写入批处理文件
                            File.WriteAllText(batchFilePath, batchContent);

                            // 启动批处理文件
                            var process = new Process();
                            ProcessStartInfo processInfo = new(
                                "cmd.exe",
                                $"/c start {batchFilePath}"
                            )
                            {
                                UseShellExecute = true,
                                CreateNoWindow = true,
                            };
                            process.StartInfo = processInfo;
                            process.Start();

                            // 关闭当前应用程序
                            MainWindowClass.Close();
                        }
                        else
                        {
                            Reminders.Reminder_Box_Show("更新失败", "red");
                            Reminders.LogsOutputting("更新失败");
                            Reminder_Interface_Show_Window.Visibility = Visibility.Collapsed;
                        }
                    };
                }
                else
                {
                    Reminder_Interface_Show_Window.Yes_CornerButten.Click += (sender, e) =>
                    {
                        Reminder_Interface_Show_Window.Visibility = Visibility.Collapsed;
                    };
                }

                Reminder_Interface_Show_Window.SubjectTextBlock.Text = subject;
                Reminder_Interface_Show_Window.TextTextBlock.Text = message;
                MainWindowClass.RemindersNavigation.Navigate(Reminder_Interface_Show_Window);
            }

            public static void Reminder_Download_Show()
            {
                if (Reminder_Download_Show_Window.Visibility == Visibility.Collapsed)
                {
                    Reminder_Download_Show_Window.Visibility = Visibility.Visible;
                }

                MainWindowClass.RemindersNavigation.Navigate(Reminder_Download_Show_Window);
            }
        }

        public static async void Update()
        {
            Reminders.Reminder_Box_Show("开始更新", "blue");
            Reminders.LogsOutputting("开始更新");

            if (
                await Downloadfiles.Downloadasync(
                    "https://cpl.chmlfrp.com/update/update.json",
                    Paths.Temp.temp_api_login
                )
            )
            {
                var JObject1 = JObject.Parse(File.ReadAllText(Paths.Temp.temp_api_login));
                string version = JObject1["version"]?.ToString();
                string subject = JObject1["subject"]?.ToString();
                string text = JObject1["text"]?.ToString();
                string url = JObject1["url"]?.ToString();

                if (version == Assembly.GetExecutingAssembly().GetName().Version.ToString())
                {
                    Reminders.Reminder_Box_Show("已是最新版本");
                    Reminders.LogsOutputting("已是最新版本");
                    return;
                }
                else
                {
                    Reminders.Reminder_Box_Show("发现新版本", "blue");

                    await Task.Delay(2000);
                    Reminders.Reminder_Interface_Show(subject, text, true, url);
                }
            }
            else
            {
                Reminders.Reminder_Box_Show("更新失败", "red");
                Reminders.LogsOutputting("更新失败");
            }
        }
    }
}

//Qusay Diaz©2024-2025
