using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using static ChmlFrp_Professional_Launcher.MainClass;

namespace ChmlFrp_Professional_Launcher.Pages
{
    public partial class LaunchPage : Page
    {
        private Process process;

        public LaunchPage()
        {
            InitializeComponent();
            new User();

            if (SignInBool)
            {
                if (
                    Downloadfiles.Download(
                        "http://cf-v2.uapis.cn/tunnel?token=" + User.usertoken,
                        Paths.Temp.temp_api_tunnel
                    )
                )
                {
                    var jsonObject = JObject.Parse(File.ReadAllText(Paths.Temp.temp_api_tunnel));

                    if (jsonObject["state"]?.ToString() == "fail")
                    {
                        return;
                    }
                    foreach (var tunnel in jsonObject["data"])
                    {
                        comboBox.Items.Add(tunnel["name"]?.ToString());
                    }
                }
            }
        }

        private string node;

        private async void Launch(object sender, RoutedEventArgs e)
        {
            LaunchButton.Click -= Launch;

            if (comboBox.Text == "")
            {
                Reminders.Reminder_Box_Show("请选择一个隧道", "red");
                LaunchButton.Click += Launch;
                return;
            }
            string frpciniFilePath = Path.Combine(Paths.IniPath, $"{comboBox.Text}.ini");

            if (!File.Exists(frpciniFilePath))
            {
                var jsonObject = JObject.Parse(File.ReadAllText(Paths.Temp.temp_api_tunnel));

                string api_tunnel_path = Path.GetTempFileName();

                foreach (var tunnel in jsonObject["data"])
                {
                    if (tunnel["name"].ToString() == comboBox.Text)
                    {
                        node = tunnel["node"].ToString();
                        break;
                    }
                }

                if (
                    await Downloadfiles.Downloadasync(
                        $"http://cf-v2.uapis.cn/tunnel_config?token={User.usertoken}&node={node}&tunnel_names={comboBox.Text}",
                        api_tunnel_path
                    )
                )
                {
                    jsonObject = JObject.Parse(File.ReadAllText(api_tunnel_path));
                    File.WriteAllText(frpciniFilePath, jsonObject["data"]?.ToString());
                }
                else
                {
                    Reminders.Reminder_Box_Show("获取API失败", "red");
                    LaunchButton.Click += Launch;
                    return;
                }
            }

            string frpclogFilePath = Path.Combine(Paths.CPLPath, "frpc.logs");

            process = new Process();

            ProcessStartInfo processInfo = new(
                "cmd.exe",
                $"/c {Paths.frpExePath} -c {frpciniFilePath} > {frpclogFilePath} 2>&1"
            ) // 命令
            {
                UseShellExecute = false,
                CreateNoWindow = true,
            }; // 配置
            process.StartInfo = processInfo; // 使用

            try
            {
                process.Start(); // 启动
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
