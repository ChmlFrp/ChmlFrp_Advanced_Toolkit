namespace CPL.Pages.ChmlFrpPages.ChmlFrpLoginedPages;

public partial class TmaPage
{
    public TmaPage()
    {
        InitializeComponent();
        InitializeApps();
    }

    private async void InitializeApps()
    {
        if (!await Downloadfiles.Downloadasync(
                "https://cf-v2.uapis.cn/tunnel?token=" + User.Usertoken,
                Paths.Temp.TempApiTunnel
            )) return;

        var jsonObject = JObject.Parse(File.ReadAllText(Paths.Temp.TempApiTunnel));

        if (jsonObject["msg"]?.ToString() != "获取隧道数据成功") return;
        var index = 1;
        PagesClass.LaunchPage.ComboBox.Items.Clear();
        foreach (var tunnel in jsonObject["data"]!)
            if (index != 6)
            {
                var tunnelid = tunnel["id"]?.ToString();
                /*var tunneltype = tunnel["type"]?.ToString();*/
                var tunnelip = tunnel["ip"]?.ToString();
                /*var tunnelnport = tunnel["nport"]?.ToString();*/
                var tunneldorp = tunnel["dorp"]!.ToString();
                var tunnelname = tunnel["name"]!.ToString();
                var tunnelstate = tunnel["state"]!.Value<bool>();

                if (tunnelstate) PagesClass.LaunchPage.ComboBox.Items.Add(tunnelname);

                var border = new Border
                {
                    Background = (Brush)FindResource("WhiteColorLight"),
                    CornerRadius = new CornerRadius(6),
                    Margin = new Thickness(2),
                    Opacity = 0.8
                };
                var tunnelTextBox = new CornerTunnelTextBox
                {
                    Margin = new Thickness(3),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    IsTrue = tunnelstate
                };
                var tunnelTextBlock1 = new CornerTextBlock
                {
                    Margin = new Thickness(5),
                    Text = tunnelname,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };
                var tunnelTextBlock2 = new CornerTextBlock
                {
                    Margin = new Thickness(5),
                    Text = $"#{tunnelid}",
                    Foreground = new SolidColorBrush(Colors.Gray),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Bottom
                };
                var tunnelTextBlock3 = new CornerTextBlock
                {
                    Margin = new Thickness(5),
                    Text = $"连接地址: {tunnelip}:{tunneldorp}",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center
                };

                tunnelTextBlock3.PreviewMouseLeftButtonDown += (_, _) =>
                    CopyToClipboard($"{tunnelip}:{tunneldorp}");

                var grid = new Grid();
                grid.Children.Add(tunnelTextBox);
                grid.Children.Add(tunnelTextBlock1);
                grid.Children.Add(tunnelTextBlock2);
                grid.Children.Add(tunnelTextBlock3);
                border.Child = grid;
                Grid.SetRow(border, (index - 1) / 2);
                Grid.SetColumn(border, (index - 1) % 2);
                MainGrid.Children.Add(border);
                index++;
            }
            else
            {
                break;
            }
    }

    private static void CopyToClipboard(string text)
    {
        Clipboard.SetText(text);
        Reminders.Reminder_Box_Show("连接地址已复制到剪贴板");
    }

    private void btnRefresh_Click(object sender, RoutedEventArgs e)
    {
        MainGrid.Children.Clear();
        InitializeApps();
        Reminders.Reminder_Box_Show("刷新成功");
    }

    private async void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        Reminders.Reminder_Box_Show("跳转中...");
        await Task.Delay(500);
        Process.Start(new ProcessStartInfo("https://preview.panel.chmlfrp.cn/tunnel/list"));
    }
}