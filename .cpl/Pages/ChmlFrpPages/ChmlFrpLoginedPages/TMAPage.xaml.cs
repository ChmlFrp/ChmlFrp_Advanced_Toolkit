using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static ChmlFrp_Professional_Launcher.MainClass;

namespace ChmlFrp_Professional_Launcher.Pages.ChmlFrpLoginPages
{
    /// <summary>
    /// TMAPage.xaml 的交互逻辑
    /// </summary>
    public partial class TMAPage : Page
    {
        public TMAPage()
        {
            InitializeComponent();
            new User();
            InitializeApps();
        }

        public async void InitializeApps()
        {
            if (
                await Downloadfiles.Downloadasync(
                    "http://cf-v2.uapis.cn/tunnel?token=" + User.usertoken,
                    Paths.Temp.temp_api_tunnel
                )
            )
            {
                var jsonObject = JObject.Parse(File.ReadAllText(Paths.Temp.temp_api_tunnel));
                if (jsonObject["msg"]?.ToString() == "获取隧道数据成功")
                {
                    int index = 1;
                    foreach (var tunnel in jsonObject["data"])
                    {
                        if (index != 6)
                        {
                            string tunnelid = tunnel["id"].ToString();
                            string tunneltype = tunnel["type"].ToString();
                            string tunnelip = tunnel["ip"].ToString();
                            string tunnelnport = tunnel["nport"].ToString();
                            string tunneldorp = tunnel["dorp"].ToString();

                            var border = new Border
                            {
                                Background = (Brush)FindResource("WhiteColorLiglt"),
                                CornerRadius = new CornerRadius(6),
                                Margin = new Thickness(2),
                                Opacity = 0.8,
                            };
                            var tunnelTextBox = new CornerTunnelTextBox
                            {
                                Margin = new Thickness(3),
                                HorizontalAlignment = HorizontalAlignment.Right,
                                VerticalAlignment = VerticalAlignment.Top,
                                IsTrue = tunnel["state"].Value<bool>(),
                            };
                            var tunnelTextBlock1 = new CornerTextBlock
                            {
                                Margin = new Thickness(5),
                                Text = tunnel["name"].ToString(),
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Top,
                            };
                            var tunnelTextBlock2 = new CornerTextBlock
                            {
                                Margin = new Thickness(5),
                                Text = $"#{tunnelid}",
                                Foreground = new SolidColorBrush(Colors.Gray),
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Bottom,
                            };
                            var tunnelTextBlock3 = new CornerTextBlock
                            {
                                Margin = new Thickness(5),
                                Text = $"连接地址: {tunnelip}:{tunneldorp}",
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Center,
                            };

                            tunnelTextBlock3.PreviewMouseLeftButtonDown += (sender, e) => CopyToClipboard($"{tunnelip}:{tunneldorp}");

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
                }
            }
        }

        private void CopyToClipboard(string text)
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
}