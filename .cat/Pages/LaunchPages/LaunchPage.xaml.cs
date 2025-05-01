using System.Collections.Generic;

namespace CPL.Pages.LaunchPages;

public partial class LaunchPage
{
    private string _node;
    private Process _process;

    public LaunchPage()
    {
        InitializeComponent();
    }

    private async void Launch(object sender, RoutedEventArgs e)
    {
        LaunchButton.Click -= Launch;
        var tunnelName = ComboBox.Text;

        if (tunnelName == "")
        {
            Reminders.Reminder_Box_Show("请选择一个隧道", "red");
            LaunchButton.Click += Launch;
            return;
        }

        var frpciniFilePath = Path.Combine(Paths.IniPath, $"{tunnelName}.ini");

        var parameters = new Dictionary<string, string>
        {
            { "token", $"{User.Usertoken}" }
        };
        
        var jObject = await Downloadfiles.GetApi("https://cf-v2.uapis.cn/tunnel", parameters);

        if (jObject == null)
        {
            Reminders.Reminder_Box_Show("获取隧道数据失败", "red");
            return;
        }

        foreach (var tunnel in jObject["data"]!)
            if (tunnel["name"]?.ToString() == tunnelName)
            {
                _node = tunnel["node"]?.ToString();
                break;
            }

        parameters = new Dictionary<string, string>
        {
            { "token", $"{User.Usertoken}" },
            {"node",$"{_node}"},
            {"tunnel_names",$"{tunnelName}"}
        };

        jObject = await Downloadfiles.GetApi("https://cf-v2.uapis.cn/tunnel_config", parameters);
        
        if (jObject == null)
        {
            Reminders.Reminder_Box_Show("获取隧道配置失败", "red");
            LaunchButton.Click += Launch;
            return;
        }
        
        File.WriteAllText(frpciniFilePath, jObject["data"]?.ToString());

        var frpclogFilePath = Path.Combine(Paths.LogPath, $"{tunnelName}.logs");

        if (Paths.IsOccupied(frpclogFilePath))
        {
            Reminders.Reminder_Box_Show("启动进程失败: 已启动隧道。", "red");

            LaunchButton.Click += Launch;
            return;
        }

        _process = new Process();

        _process.StartInfo = new(
            "cmd.exe",
            $"/c chcp 65001 & {Paths.FrpExePath} -c {frpciniFilePath} > {frpclogFilePath}"
        )
        {
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _process.Start();
        Reminders.Reminder_Box_Show($"{tunnelName}启动成功");
        LaunchButton.Click += Launch;
    }
}