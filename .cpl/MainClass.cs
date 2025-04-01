using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ChmlFrp_Professional_Launcher.Pages.ChmlFrpPages;
using ChmlFrp_Professional_Launcher.Pages.LaunchPages;
using ChmlFrp_Professional_Launcher.Pages.ReminderPages;
using Newtonsoft.Json.Linq;
using Microsoft.Win32;
using System.Linq;
using System.Windows.Media.Imaging;

namespace ChmlFrp_Professional_Launcher
{
    public partial class App
    {
        static App()
        {
            MainClass.Initialize.InitializeFirst();
        }
    }

    internal static class MainClass
    {
        public static MainWindow MainWindowClass;

        internal static class PagesClass
        {
            public static ChmlFrphomePage ChmlFrpHomePage = new();
            public static readonly LaunchPage LaunchPage = new();
        }

        private static bool IsProcess(string name)
        {
            return Process.GetProcessesByName(name).Length > 1;
        }

        public static volatile bool SignInBool;

        internal class Paths
        {
            //定义路径
            private static readonly string DirectoryPath = Directory.GetCurrentDirectory();

            public static string IniPath;
            public static string FrpExePath;
            public static string CplPath;
            public static string PicturesPath;
            public static string LogfilePath;

            public Paths()
            {
                CplPath = Path.Combine(DirectoryPath, "CPL");
                PicturesPath = Path.Combine(CplPath, "Pictures");
                IniPath = Path.Combine(CplPath, "Ini");
                LogfilePath = Path.Combine(CplPath, "Debug.logs");
                FrpExePath = Path.Combine(CplPath, "frpc.exe");
                new Temp();
            }

            internal class Temp
            {
                public static string TempApiTunnel;
                public static string TempApiLogin;
                public static string TempApiUser;
                public static string TempUserImage;

                public Temp()
                {
                    TempApiTunnel = Path.GetTempFileName();
                    TempApiLogin = Path.GetTempFileName();
                    TempApiUser = Path.GetTempFileName();
                    TempUserImage = Path.GetTempFileName();
                }
            }
        }

        internal class User
        {
            public static string Username;
            public static string Password;
            public static string Usertoken;

            public User()
            {
                var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\\ChmlFrp");
                if (key == null)
                {
                    Registry.CurrentUser.CreateSubKey(@"SOFTWARE\\ChmlFrp");
                    return;
                }

                Username = key.GetValue("username")?.ToString();
                Password = key.GetValue("password")?.ToString();
                Usertoken = key.GetValue("usertoken")?.ToString();
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
                    Reminders.LogsOutputting($"下载失败：文件占用或网络错误?&url={url}");
                    return false;
                }

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
                    Reminders.LogsOutputting($"下载失败：文件占用或网络错误?&url={url}");
                    return false;
                }

                return true;
            }

            public static bool GetApItoLogin(bool remind = true)
            {
                new User();

                if (Download(
                        $"https://cf-v2.uapis.cn/login?username={User.Username}&password={User.Password}",
                        Paths.Temp.TempApiLogin
                    ))
                {
                    var msg = JObject
                        .Parse(File.ReadAllText(Paths.Temp.TempApiLogin))["msg"]
                        ?.ToString();
                    Reminders.LogsOutputting("API提醒：" + msg);
                    if (msg == "登录成功")
                    {
                        if (remind)
                            Reminders.Reminder_Box_Show(msg);
                        SignInBool = true;
                        return true;
                    }
                    else
                    {
                        if (remind)
                            Reminders.Reminder_Box_Show(msg, "red");
                    }
                }
                else
                {
                    if (remind)
                        Reminders.Reminder_Box_Show("网络错误", "red");
                }

                SignInBool = false;
                return false;
            }
        }

        internal static class Reminders
        {
            private static readonly ReminderBoxShow ReminderBoxShowWindow = new();
            private static readonly ReminderInterfaceShow ReminderInterfaceShowWindow = new();
            private static readonly ReminderDownloadShow ReminderDownloadShowWindow = new();

            public static void LogsOutputting(string logEntry)
            {
                logEntry = $"[{DateTime.Now}] " + logEntry;
                Console.WriteLine(logEntry);
                File.AppendAllText(Paths.LogfilePath, logEntry + Environment.NewLine);
            }

            public static void Reminder_Box_Show(string message, string color = "green")
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    LogsOutputting("提醒消息不能为空或全为空格");
                    return;
                }

                if (ReminderBoxShowWindow.Visibility == Visibility.Collapsed)
                    ReminderBoxShowWindow.Visibility = Visibility.Visible;

                if (color == "green")
                {
                    ReminderBoxShowWindow.RemindersBorder.Background = new SolidColorBrush(
                        Colors.LimeGreen
                    );
                }
                else if (color == "blue")
                {
                    ReminderBoxShowWindow.RemindersBorder.Background = new SolidColorBrush(
                        Colors.DodgerBlue
                    );
                }
                else if (color == "red")
                {
                    ReminderBoxShowWindow.RemindersBorder.Background = new SolidColorBrush(
                        Colors.Red
                    );
                }
                else if (color == "yellow")
                {
                    ReminderBoxShowWindow.RemidingTextBlock.Foreground = new SolidColorBrush(
                        Colors.Green
                    );
                    ReminderBoxShowWindow.RemindersBorder.Background = new SolidColorBrush(
                        Colors.Yellow
                    );
                }

                ReminderBoxShowWindow.RemidingTextBlock.Text = message;
                MainWindowClass.RemindersNavigationTwo.Navigate(ReminderBoxShowWindow);
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

                if (ReminderInterfaceShowWindow.Visibility == Visibility.Collapsed)
                    ReminderInterfaceShowWindow.Visibility = Visibility.Visible;

                ReminderInterfaceShowWindow.Yes_CornerButten.IsSelected = true;

                if (isUpdate)
                {
                    ReminderInterfaceShowWindow.Yes_CornerButten.Content = "更新";
                    ReminderInterfaceShowWindow.Yes_CornerButten.Click += async (_, _) =>
                    {
                        ReminderInterfaceShowWindow.Visibility = Visibility.Collapsed;

                        await Task.Delay(1000);

                        var exe = Path.Combine(
                            Paths.CplPath,
                            "ChmlFrp_Professional_Launcher.exe"
                        );

                        if (Downloadfiles.Download(url, exe))
                        {
                            Reminder_Box_Show("下载成功");
                            LogsOutputting("下载成功");

                            var batchFilePath = Path.Combine(Paths.CplPath, "update.bat");
                            var processPath = Process.GetCurrentProcess().MainModule?.FileName;
                            var batchContent =
                                $@"
                                @echo off
                                timeout /t 3 /nobreak
                                move /y ""{exe}"" ""{processPath}""
                                start """" ""{processPath}""
                                exit
                                ";

                            File.WriteAllText(batchFilePath, batchContent);

                            var process = new Process();
                            ProcessStartInfo processInfo = new(
                                "cmd.exe",
                                $"/c start {batchFilePath}"
                            )
                            {
                                UseShellExecute = true,
                                CreateNoWindow = true
                            };
                            process.StartInfo = processInfo;
                            process.Start();

                            MainWindowClass.Close();
                        }
                        else
                        {
                            Reminder_Box_Show("更新失败", "red");
                            LogsOutputting("更新失败");
                            ReminderInterfaceShowWindow.Visibility = Visibility.Collapsed;
                        }
                    };
                }
                else
                {
                    ReminderInterfaceShowWindow.Yes_CornerButten.Click += (_, _) =>
                    {
                        ReminderInterfaceShowWindow.Visibility = Visibility.Collapsed;
                    };
                }

                ReminderInterfaceShowWindow.SubjectTextBlock.Text = subject;
                ReminderInterfaceShowWindow.TextTextBlock.Text = message;
                MainWindowClass.RemindersNavigation.Navigate(ReminderInterfaceShowWindow);
            }

            public static void Reminder_Download_Show()
            {
                if (ReminderDownloadShowWindow.Visibility == Visibility.Collapsed)
                    ReminderDownloadShowWindow.Visibility = Visibility.Visible;

                MainWindowClass.RemindersNavigation.Navigate(ReminderDownloadShowWindow);
            }
        }

        internal static class Initialize
        {
            public static void InitializeNext()
            {
                IsUpdate();
                SetImage();
                IsAprilFoolsDay();
                if (!File.Exists(Paths.FrpExePath)) Reminders.Reminder_Download_Show();
            }

            public static void InitializeFirst()
            {
                new Paths();

                // 检测是否有两个ChmlFrp Professional Launcher进程
                if (IsProcess(Process.GetCurrentProcess().ProcessName))
                {
                    MessageBox.Show(
                        "已存在进程，请关闭当前进程。",
                        "请关闭当前进程",
                        MessageBoxButton.OK,
                        MessageBoxImage.Stop
                    );

                    MainWindowClass.Close();
                    return;
                }

                try
                {
                    //检测是否有相关配置文件
                    if (!File.Exists(Paths.CplPath))
                        Directory.CreateDirectory(Paths.CplPath);
                    if (!File.Exists(Paths.PicturesPath))
                        Directory.CreateDirectory(Paths.PicturesPath);
                    if (File.Exists(Path.Combine(Paths.CplPath, "update.bat")))
                        File.Delete(Path.Combine(Paths.CplPath, "update.bat"));
                    if (!File.Exists(Paths.IniPath))
                        Directory.CreateDirectory(Paths.IniPath);
                    if (!File.Exists(Paths.LogfilePath))
                        File.WriteAllText(Paths.LogfilePath, string.Empty);
                }
                catch
                {
                    MessageBox.Show(
                        "配置文件创建失败，请检查权限。",
                        "配置文件创建失败",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );

                    MainWindowClass.Close();
                }
            }

            private static async void IsUpdate()
            {
                Reminders.Reminder_Box_Show("开始更新", "blue");
                Reminders.LogsOutputting("开始更新");

                if (
                    await Downloadfiles.Downloadasync(
                        "https://cpl.chmlfrp.com/update/update.json",
                        Paths.Temp.TempApiLogin
                    )
                )
                {
                    var jObject = JObject.Parse(File.ReadAllText(Paths.Temp.TempApiLogin));
                    var version = jObject["version"]?.ToString();

                    if (version == Assembly.GetExecutingAssembly().GetName().Version.ToString())
                    {
                        Reminders.Reminder_Box_Show("已是最新版本");
                        Reminders.LogsOutputting("已是最新版本");
                    }
                    else
                    {
                        Reminders.Reminder_Box_Show("发现新版本", "blue");
                        var subject = jObject["subject"]?.ToString();
                        var text = jObject["text"]?.ToString();
                        var url = jObject["url"]?.ToString();
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

            private static void IsAprilFoolsDay()
            {
                if (DateTime.Today is not { Month: 4, Day: 1 }) return;
                var resourceDictionary = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/ChmlFrp Professional Launcher;component/Themes/Theme.xaml")
                };
                if (resourceDictionary["ThemeColor"] is SolidColorBrush themeColorBrush)
                    themeColorBrush.Color = Colors.LightGreen;
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
                MainWindowClass.BlankPage.TextBlock.Text = "愚人节快乐";
                MainWindowClass.BlankPage.TextBlock.Foreground = new SolidColorBrush(Colors.LightGreen);
            }

            private static void SetImage()
            {
                var imageFiles = Directory
                    .GetFiles(Paths.PicturesPath, "*.*")
                    .Where(file =>
                        file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                        || file.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                    )
                    .ToArray();

                if (imageFiles.Length <= 0) return;

                Random random = new();
                var randomImage = imageFiles[random.Next(imageFiles.Length)];
                MainWindowClass.Imagewallpaper.ImageSource = new BitmapImage(
                    new Uri(randomImage, UriKind.RelativeOrAbsolute)
                );
                MainWindowClass.Imagewallpaper.Stretch = Stretch.UniformToFill;
            }
        }
    }
}

//Qusay Diaz©2024-2025