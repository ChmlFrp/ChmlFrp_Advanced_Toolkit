using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using static ChmlFrp_Professional_Launcher.MainClass;

namespace ChmlFrp_Professional_Launcher.Pages.LaunchPages
{
    public partial class LaunchPage : Page
    {
        private Process _process;

        public LaunchPage()
        {
            InitializeComponent();
        }

        private string _node;

        private async void Launch(object sender, RoutedEventArgs e)
        {
            LaunchButton.Click -= Launch;

            if (comboBox.Text == "")
            {
                Reminders.Reminder_Box_Show("请选择一个隧道", "red");
                LaunchButton.Click += Launch;
                return;
            }

            var frpciniFilePath = Path.Combine(Paths.IniPath, $"{comboBox.Text}.ini");

            if (!File.Exists(frpciniFilePath))
            {
                var jsonObject = JObject.Parse(File.ReadAllText(Paths.Temp.TempApiTunnel));

                var apiTunnelPath = Path.GetTempFileName();

                foreach (var tunnel in jsonObject["data"]!)
                    if (tunnel["name"]?.ToString() == comboBox.Text)
                    {
                        _node = tunnel["node"]?.ToString();
                        break;
                    }

                if (
                    await Downloadfiles.Downloadasync(
                        $"http://cf-v2.uapis.cn/tunnel_config?token={User.Usertoken}&node={_node}&tunnel_names={comboBox.Text}",
                        apiTunnelPath
                    )
                )
                {
                    jsonObject = JObject.Parse(File.ReadAllText(apiTunnelPath));
                    File.WriteAllText(frpciniFilePath, jsonObject["data"]?.ToString());
                }
                else
                {
                    Reminders.Reminder_Box_Show("获取API失败", "red");
                    LaunchButton.Click += Launch;
                    return;
                }
            }

            var frpclogFilePath = Path.Combine(Paths.CplPath, "frpc.logs");

            _process = new Process();

            ProcessStartInfo processInfo = new(
                    "cmd.exe",
                    $"/c {Paths.FrpExePath} -c {frpciniFilePath} > {frpclogFilePath} 2>&1"
                ) // 命令
                {
                    UseShellExecute = false,
                    CreateNoWindow = true
                }; // 配置
            _process.StartInfo = processInfo; // 使用

            try
            {
                _process.Start(); // 启动
            }
            catch (Exception ex)
            {
                Reminders.Reminder_Box_Show($"启动进程失败: {ex.Message}", "red");

                LaunchButton.Click += Launch;
                return;
            }

            Reminders.Reminder_Box_Show("启动成功", "green");
            LaunchButton.Click += Launch;
        }
    }
}