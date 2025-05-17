using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using CAT.Views.Windows.Styles;

namespace CAT.Views.Pages.ChmlFrpPages;

public partial class TunnelPage
{
    private const int TunnelsPerPage = 4;
    private int _currentPage = 1;
    private List<JToken> _tunnels;

    public TunnelPage()
    {
        InitializeComponent();
        InitializeApps();
    }

    private async void InitializeApps()
    {
        var parameters = new Dictionary<string, string>
        {
            { "token", $"{User.Usertoken}" }
        };

        var jObject = await Api.GetApi("https://cf-v2.uapis.cn/tunnel", parameters);

        if (jObject == null || jObject["msg"]?.ToString() != "获取隧道数据成功")
        {
            Reminders.Reminder_Box_Show("获取隧道数据失败", "red");
            return;
        }

        _tunnels = jObject["data"]!.ToList();
        DisplayTunnels();
    }

    private void DisplayTunnels()
    {
        ViewsClass.LaunchPage.TunnelComboBox.Items.Clear();

        foreach (var tunnel in _tunnels) ViewsClass.LaunchPage.TunnelComboBox.Items.Add(tunnel["name"]!.ToString());

        MainGrid.Children.Clear();

        var displayTunnels = _tunnels.Skip((_currentPage - 1) * TunnelsPerPage).Take(TunnelsPerPage);
        var index = 1;

        foreach (var tunnel in displayTunnels)
        {
            var tunnelid = tunnel["id"]?.ToString();
            var tunnelip = tunnel["ip"]?.ToString();
            var tunneldorp = tunnel["dorp"]!.ToString();
            var tunnelname = tunnel["name"]!.ToString();
            var tunnelstate = tunnel["state"]!.Value<bool>();
            var tunnelPortType = tunnel["type"]?.ToString();

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
                IsTrue = tunnelstate,
                Tag = tunnelPortType
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
                IsEnabled = false,
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

        UpdatePaginationButtons();
    }

    private void UpdatePaginationButtons()
    {
        BtnPrevious.IsEnabled = _currentPage > 1;
        BtnNext.IsEnabled = _currentPage * TunnelsPerPage < _tunnels.Count;
    }

    private static void CopyToClipboard(string text)
    {
        try
        {
            Clipboard.SetText(text);
            Reminders.Reminder_Box_Show("连接地址已复制到剪贴板");
        }
        catch
        {
            // ignored
        }
    }

    private void btnRefresh_Click(object sender, RoutedEventArgs e)
    {
        MainGrid.Children.Clear();
        _currentPage = 1;
        InitializeApps();
        Reminders.Reminder_Box_Show("刷新成功");
    }

    private void btnPrevious_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPage <= 1) return;
        _currentPage--;
        DisplayTunnels();
    }

    private void btnNext_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPage * TunnelsPerPage >= _tunnels.Count) return;
        _currentPage++;
        DisplayTunnels();
    }

    private async void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        Reminders.Reminder_Box_Show("跳转中...");
        await Task.Delay(500);
        Process.Start(new ProcessStartInfo("https://panel.chmlfrp.cn/tunnel/list"));
    }
}