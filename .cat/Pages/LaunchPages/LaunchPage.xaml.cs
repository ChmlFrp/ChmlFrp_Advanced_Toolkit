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

        var jsonObject = JObject.Parse(File.ReadAllText(Paths.Temp.TempApiTunnel));

        var apiTunnelPath = Path.GetTempFileName();

        foreach (var tunnel in jsonObject["data"]!)
            if (tunnel["name"]?.ToString() == tunnelName)
            {
                _node = tunnel["node"]?.ToString();
                break;
            }

        if (
            await Downloadfiles.Downloadasync(
                $"https://cf-v2.uapis.cn/tunnel_config?token={User.Usertoken}&node={_node}&tunnel_names={tunnelName}",
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