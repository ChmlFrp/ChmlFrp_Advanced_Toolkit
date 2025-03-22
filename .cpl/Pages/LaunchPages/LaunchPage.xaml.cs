using System.Windows;
using System.Windows.Controls;

namespace ChmlFrp_Professional_Launcher.Pages
{
    public partial class LaunchPage : Page
    {
        //MainWindow MainWindow = Application.Current.MainWindow as MainWindow;

        //private Process process;

        public LaunchPage()
        {
            InitializeComponent();
            //new MainClass.User();

            //if (
            //    MainClass.Downloadfiles.Download(
            //        "http://cf-v2.uapis.cn/tunnel?token=" + MainClass.User.usertoken,
            //        MainClass.Paths.Temp.temp_api_tunnel
            //    )
            //)
            //{
            //    string jsonContent = File.ReadAllText(MainClass.Paths.Temp.temp_api_tunnel);
            //    var jsonObject = JObject.Parse(jsonContent);

            //    if (jsonObject["state"]?.ToString() == "fail")
            //    {
            //        return;
            //    }
            //    foreach (var tunnel in jsonObject["data"])
            //    {
            //        comboBox.Items.Add(tunnel["name"]?.ToString());
            //    }
            //}
        }

        //int i;

        private void Launch(object sender, RoutedEventArgs e)
        {
            MainClass.Reminders.Reminder_Box_Show("正在施工...", "yellow");
            return;

            //LaunchButton.Click -= Launch;
            //string frpciniFilePath = "";

            //i++;
            //i = (i == 6) ? 1 : i;
            //string logFilePath = Path.Combine(MainClass.Paths.CPLPath, $"{i}.logs");

            //process = new Process();

            //ProcessStartInfo processInfo = new(
            //    "cmd.exe",
            //    $"/c {MainClass.Paths.frpExePath} -c {frpciniFilePath} > {logFilePath} 2>&1"
            //) // 命令
            //{
            //    UseShellExecute = false,
            //    CreateNoWindow = true,
            //}; // 配置
            //process.StartInfo = processInfo; // 使用

            //try
            //{
            //    process.Start(); // 启动
            //}
            //catch (Exception ex)
            //{
            //    MainClass.Reminders.Reminder_Box_Show($"启动进程失败: {ex.Message}", "red");

            //    LaunchButton.Click += Launch;
            //    return;
            //}
            //MainClass.Reminders.Reminder_Box_Show("启动成功", "green");

            //LaunchButton.Click += Killfrp;
            //LaunchButton.Content = "关闭FRPC";
        }

        //private void Killfrp(object sender, RoutedEventArgs e)
        //{
        //    LaunchButton.Click -= Killfrp;

        //    if (process.HasExited)
        //        MainClass.Reminders.Reminder_Box_Show("进程已退出", "red");
        //    else
        //    {
        //        process.Kill(); // 关闭
        //        process.Dispose();
        //        MainClass.Reminders.Reminder_Box_Show("关闭成功", "green");
        //    }

        //    LaunchButton.Click += Launch;
        //    LaunchButton.Content = "启动FRPC";
        //}
    }
}
