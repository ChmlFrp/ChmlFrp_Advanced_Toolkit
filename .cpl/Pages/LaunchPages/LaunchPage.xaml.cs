using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ChmlFrp_Professional_Launcher.Pages
{
    public partial class LaunchPage : Page
    {
        private Process process;

        public LaunchPage()
        {
            InitializeComponent();
            new MainClass.User();

            if (
                MainClass.Downloadfiles.Download(
                    "http://cf-v2.uapis.cn/tunnel?token=" + MainClass.User.usertoken,
                    MainClass.Paths.Temp.temp_api_tunnel
                )
            )
            {
                var jsonObject = JObject.Parse(File.ReadAllText(MainClass.Paths.Temp.temp_api_tunnel));

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

        string node;

        private async void Launch(object sender, RoutedEventArgs e)
        {
            LaunchButton.Click -= Launch;

            if (comboBox.Text == "")
            {
                MainClass.Reminders.Reminder_Box_Show("请选择一个隧道", "red");
                LaunchButton.Click += Launch;
                return;
            }
            string frpciniFilePath = Path.Combine(MainClass.Paths.IniPath, $"{comboBox.Text}.ini");

            if(!File.Exists(frpciniFilePath))
            {
                var jsonObject = JObject.Parse(File.ReadAllText(MainClass.Paths.Temp.temp_api_tunnel));

                string api_tunnel_path = Path.GetTempFileName();

                foreach (var tunnel in jsonObject["data"])
                {
                    if (tunnel["name"].ToString() == comboBox.Text)
                    {
                        node = tunnel["node"].ToString();
                        break;
                    }
                }

                if (await MainClass.Downloadfiles.Downloadasync($"http://cf-v2.uapis.cn/tunnel_config?token={MainClass.User.usertoken}&node={node}&tunnel_names={comboBox.Text}", api_tunnel_path))
                {
                    jsonObject = JObject.Parse(File.ReadAllText(api_tunnel_path));
                    File.WriteAllText(frpciniFilePath, jsonObject["data"]?.ToString());
                }
                else
                {
                    MainClass.Reminders.Reminder_Box_Show("获取API失败", "red");
                    LaunchButton.Click += Launch;
                    return;
                }
            }

            string frpclogFilePath = Path.Combine(MainClass.Paths.CPLPath, "frpc.logs");

            process = new Process();

            ProcessStartInfo processInfo = new(
                "cmd.exe",
                $"/c {MainClass.Paths.frpExePath} -c {frpciniFilePath} > {frpclogFilePath} 2>&1"
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
                MainClass.Reminders.Reminder_Box_Show($"启动进程失败: {ex.Message}", "red");

                LaunchButton.Click += Launch;
                return;
            }
            MainClass.Reminders.Reminder_Box_Show("启动成功", "green");

            LaunchButton.Click += Launch;
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
