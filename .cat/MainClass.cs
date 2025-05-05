using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Microsoft.Win32;

namespace CAT;

public abstract class MainClass
{
    public static MainWindow MainWindowClass;

    public static volatile bool SignInBool;

    private static bool IsProcess(string name)
    {
        return Process.GetProcessesByName(name).Length > 1;
    }

    internal static class PagesClass
    {
        public static ChmlFrpHomePage ChmlFrpHomePage;
        public static LaunchPage LaunchPage;
    }

    public abstract class Paths
    {
        //定义路径
        public static readonly string DataPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ChmlFrp");

        public static readonly string IniPath = Path.Combine(DataPath, "Inis");
        public static readonly string LogPath = Path.Combine(DataPath, "Logs");
        public static readonly string Plugin = Path.Combine(DataPath, "Plugins");
        public static readonly string FrpExePath = Path.Combine(DataPath, "frpc.exe");
        public static readonly string PicturesPath = Path.Combine(DataPath, "Pictures");
        public static readonly string InifilePath = Path.Combine(DataPath, "CAT_Config.ini");
        public static readonly string LogfilePath = Path.Combine(DataPath, "Debug-CAT.logs");

        public static bool IsOccupied(string filePath)
        {
            if (!File.Exists(filePath)) return false;

            FileStream stream = null;
            try
            {
                stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
                return false;
            }
            catch
            {
                return true;
            }
            finally
            {
                stream?.Close();
            }
        }
    }

    public static class User
    {
        private static readonly RegistryKey Key =
            Registry.CurrentUser.CreateSubKey(@"SOFTWARE\\ChmlFrp", true);

        public static string Username;
        public static string Password;
        public static string Usertoken;

        static User()
        {
            Load();
        }

        private static void Load()
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

    public static class Downloadfiles
    {
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

        public static async Task<JObject> GetApi(string url, Dictionary<string, string> parameters = null)
        {
            if (parameters != null)
                url = $"{url}?{string.Join("&", parameters.Select(pair => $"{pair.Key}={pair.Value}"))}";

            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var jObject = JObject.Parse(await response.Content.ReadAsStringAsync());
                return jObject;
            }
            catch (Exception ex)
            {
                Reminders.LogsOutputting($"请求错误: {ex.Message}");
                return null;
            }
        }

        public static async Task<bool> GetApItoLogin(bool isRemind = true, string name = "", string password = "")
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
            {
                password = User.Password;
                name = User.Username;
            }

            var parameters = new Dictionary<string, string>
            {
                { "username", $"{name}" },
                { "password", $"{password}" }
            };

            var jObject = await GetApi("https://cf-v2.uapis.cn/login", parameters);

            if (jObject == null)
            {
                if (isRemind) Reminders.Reminder_Box_Show("网络错误", "red");
                return false;
            }

            var msg = jObject["msg"]?.ToString();
            Reminders.LogsOutputting("API提醒：" + msg);
            if (msg != "登录成功")
            {
                if (isRemind) Reminders.Reminder_Box_Show(msg, "red");
                return false;
            }

            if (isRemind) Reminders.Reminder_Box_Show(msg);

            User.Save(name, password, jObject["data"]?["usertoken"]?.ToString());
            SignInBool = true;
            return true;
        }
    }

    public static class Reminders
    {
        private static readonly ReminderBoxShow Rbsw = new();
        private static readonly ReminderInterfaceShow Risw = new();
        private static readonly ReminderDownloadShow Rdsw = new();

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

    public static void LoadResources(string typename, Grid pluginGrid, Border pluginBorder)
    {
        var dependencyDlls = Directory.GetFiles(Paths.Plugin, "*.dll")
            .Where(f => !Path.GetFileName(f).Contains("Plugin"))
            .ToList();

        var pluginDlls = Directory.GetFiles(Paths.Plugin, "*.dll")
            .Where(f => Path.GetFileName(f).Contains("Plugin"))
            .ToList();

        if (pluginDlls.Count == 0) return;

        foreach (var dep in dependencyDlls) Assembly.LoadFrom(dep);
        foreach (var plugin in pluginDlls)
            try
            {
                var asm = Assembly.LoadFrom(plugin);
                var type = asm.GetType(typename);
                if (type == null || !typeof(UserControl).IsAssignableFrom(type)) continue;
                pluginBorder.Visibility = Visibility.Visible;
                pluginGrid.Children.Add((UserControl)Activator.CreateInstance(type));
            }
            catch (Exception ex)
            {
                Reminders.Reminder_Box_Show($"加载插件时发生错误:{ex.Message}", "red");
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
            // 检测是否有两个ChmlFrp Advanced Toolkit进程
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
                Directory.CreateDirectory(Paths.DataPath);
                Directory.CreateDirectory(Paths.LogPath);
                Directory.CreateDirectory(Paths.PicturesPath);
                Directory.CreateDirectory(Paths.IniPath);
                Directory.CreateDirectory(Paths.Plugin);
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

        private static async void IsUpdate(bool isRemind = false)
        {
            if (isRemind) Reminders.Reminder_Box_Show("开始更新", "blue");
            Reminders.LogsOutputting("开始更新");

            var jObject = await Downloadfiles.GetApi("http://cat.chmlfrp.com/update/update.json");

            if (jObject == null)
            {
                if (isRemind) Reminders.Reminder_Box_Show("更新失败", "red");
                Reminders.LogsOutputting("更新失败");
            }

            if (jObject!["version"]?.ToString() == Assembly.GetExecutingAssembly().GetName().Version.ToString())
            {
                if (isRemind) Reminders.Reminder_Box_Show("已是最新版本");
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

        private static void IsAprilFoolsDay()
        {
            if (DateTime.Today is not { Month: 4, Day: 1 }) return;
            var resourceDictionary = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/ChmlFrp Advanced Toolkit;component/Themes/Theme.xaml")
            };
            if (resourceDictionary["ThemeColor"] is SolidColorBrush themeColorBrush)
                themeColorBrush.Color = Colors.LightGreen;
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
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