using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace CAT;

public partial class LaunchingUserControl
{
    private int _i;

    public LaunchingUserControl()
    {
        InitializeComponent();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        _i++;
        switch (_i)
        {
            case 1:
            {
                Button.Content = "你点我干啥？😫";
                break;
            }
            case 2:
            {
                Button.Content = "你再点试试？😠";
                break;
            }
            case 3:
            {
                Button.Content = "你再点！😡";
                break;
            }
            case 4:
            {
                Button.Content = "你再点！!😡😡";
                break;
            }
            case 5:
            {
                Button.Content = "你再点！！！🤬";
                break;
            }
            case 6:
            {
                Button.Content = "你干嘛！哎呦喂。😫";
                await Task.Delay(1000);
                Process.Start(
                    new ProcessStartInfo("https://www.bilibili.com/video/BV1GJ411x7h7")
                );
                _i = 0;
                break;
            }
            default:
            {
                MainClass.Reminders.Reminder_Interface_Show("关于此插件",
                    "我就是启动器作者，此插件根据\nhttps://github.com/Qianyiaz/ClickMe\n此项目移植而来。");
                break;
            }
        }
    }
}