using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Microsoft.Win32;

namespace CPL;

internal static class MainClass
{
    public static MainWindow MainWindowClass;

    public static volatile bool SignInBool;

    private static bool IsProcess(string name)
    {
        return Process.GetProcessesByName(name).Length > 1;
    }

    internal static class PagesClass
    {
        public static ChmlFrphomePage ChmlFrpHomePage;
        public static readonly LaunchPage LaunchPage = new();
    }

    public abstract class Paths
    {
        //定义路径
        private static readonly string DirectoryPath = Directory.GetCurrentDirectory();

        public static readonly string CplPath = Path.Combine(DirectoryPath, "CPL");
        public static readonly string IniPath = Path.Combine(CplPath, "Ini");
        public static readonly string FrpExePath = Path.Combine(CplPath, "frpc.exe");
        public static readonly string PicturesPath = Path.Combine(CplPath, "Pictures");
        public static readonly string LogfilePath = Path.Combine(CplPath, "Debug.logs");

        public abstract class Temp
        {
            public static readonly string TempApiTunnel = Path.GetTempFileName();
            public static readonly string TempApiLogin = Path.GetTempFileName();
            public static readonly string TempApiUser = Path.GetTempFileName();
            public static string TempUserImage;
        }
    }

    public abstract class User
    {
        private static readonly RegistryKey Key =
            Registry.CurrentUser.CreateSubKey(@"SOFTWARE\\ChmlFrp", true);
        public static string Username;
        public static string Password;
        public static string Usertoken;

        public static void Load()
        {
            Username = Key.GetValue("username")?.ToString();
            Password = Key.GetValue("password")?.ToString();
            Usertoken = Key.GetValue("usertoken")?.ToString();
        }

        public static void Save(string username, string password, string usertoken)
        {
            Key.SetValue("username", username);
            Key.SetValue("password", password);
            Key.SetValue("usertoken", usertoken);
            Load();
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

        public static bool GetApItoLogin(bool remind = true, string name = "", string password = "")
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
            {
                password = User.Password;
                name = User.Username;
            }
            
            if (!Download(
                    $"https://cf-v2.uapis.cn/login?username={name}&password={password}",
                    Paths.Temp.TempApiLogin
                ))
            {
                if (remind) Reminders.Reminder_Box_Show("网络错误", "red");
                return false;
            }
            
            var jObject = JObject.Parse(File.ReadAllText(Paths.Temp.TempApiLogin));
            var msg = jObject["msg"]?.ToString();
            
            Reminders.LogsOutputting("API提醒：" + msg);
            if (msg != "登录成功")
            {
                if (remind) Reminders.Reminder_Box_Show(msg,"red");
                return false;
            }
            if (remind) Reminders.Reminder_Box_Show(msg);

            User.Save(name, password, jObject["data"]?["usertoken"]?.ToString());
            SignInBool = true;
            return true;
        }
    }

    internal static class Reminders
    {
        private static readonly ReminderBoxShow Rbsw = new();
        private static readonly ReminderInterfaceShow Risw = new();
        private static readonly ReminderDownloadShow Rdsw = new();
        private static bool _first = true;


        public static void LogsOutputting(string logEntry)
        {
            logEntry = $"[{DateTime.Now}] " + logEntry;
            if (_first)
            {
                _first = false;
                File.WriteAllText(Paths.LogfilePath, string.Empty);
            }
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

            if (Rbsw.Visibility == Visibility.Collapsed)
                Rbsw.Visibility = Visibility.Visible;

            switch (color)
            {
                case "green":
                    Rbsw.RemindersBorder.Background = new SolidColorBrush(
                        Colors.LimeGreen
                    );
                    break;
                case "blue":
                    Rbsw.RemindersBorder.Background = new SolidColorBrush(
                        Colors.DodgerBlue
                    );
                    break;
                case "red":
                    Rbsw.RemindersBorder.Background = new SolidColorBrush(
                        Colors.Red
                    );
                    break;
                case "yellow":
                    Rbsw.RemidingTextBlock.Foreground = new SolidColorBrush(
                        Colors.Green
                    );
                    Rbsw.RemindersBorder.Background = new SolidColorBrush(
                        Colors.Yellow
                    );
                    break;
            }

            Rbsw.RemidingTextBlock.Text = message;
            MainWindowClass.RemindersNavigationTwo.Navigate(Rbsw);
        }

        public static void Reminder_Interface_Show(
            string subject,
            string message,
            bool isUpdate = false
        )
        {
            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(message))
            {
                LogsOutputting("提醒消息不能为空或全为空格");
                return;
            }

            if (Risw.Visibility == Visibility.Collapsed)
                Risw.Visibility = Visibility.Visible;

            Risw.YesCornerButten.IsSelected = true;

            if (isUpdate)
            {
                Risw.YesCornerButten.Content = "更新";
                Risw.YesCornerButten.Click += Risw.Update;
            }
            else
            {
                Risw.YesCornerButten.Click += Risw.Close;
            }

            Risw.SubjectTextBlock.Text = subject;
            Risw.TextTextBlock.Text = message;
            MainWindowClass.RemindersNavigation.Navigate(Risw);
        }

        public static void Reminder_Download_Show()
        {
            if (Rdsw.Visibility == Visibility.Collapsed)
                Rdsw.Visibility = Visibility.Visible;

            MainWindowClass.RemindersNavigation.Navigate(Rdsw);
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
            }

            try
            {
                User.Load();
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
                if (jObject["version"]?.ToString() == Assembly.GetExecutingAssembly().GetName().Version.ToString())
                {
                    Reminders.Reminder_Box_Show("已是最新版本");
                    Reminders.LogsOutputting("已是最新版本");
                }
                else
                {
                    Reminders.Reminder_Box_Show("发现新版本", "blue");
                    await Task.Delay(2000);
                    Reminders.Reminder_Interface_Show(jObject["subject"]?.ToString(), jObject["text"]?.ToString(),
                        true);
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

//Qusay Diaz©2024-2025