using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CAT.Views.Pages.ChmlFrpPages;

public partial class UserinfoPage
{
    private readonly DispatcherTimer _timer;

    private int _i;

    public UserinfoPage()
    {
        InitializeComponent();
        InitializeApps(null, null);

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(30)
        };
        _timer.Tick += InitializeApps;
        _timer.Start();
    }

    private async void InitializeApps(object sender, EventArgs eventArgs)
    {
        var parameters = new Dictionary<string, string>
        {
            { "token", $"{User.Usertoken}" }
        };

        var jObject = await Api.GetApi("https://cf-v2.uapis.cn/userinfo", parameters);

        if (jObject == null)
        {
            Reminders.Reminder_Box_Show("用户信息加载失败", "red");
            return;
        }

        var tempUserImage = Path.GetTempFileName();

        await Api.Downloadasync(
            jObject["data"]?["userimg"]?.ToString(),
            tempUserImage
        );

        UserImage.ImageSource = new BitmapImage(new Uri(tempUserImage));
        UsernameBlock.Text = jObject["data"]?["username"]?.ToString();
        IntegralCard.Text = jObject["data"]?["integral"]?.ToString()!;
        EmailCard.Text = jObject["data"]?["email"]?.ToString()!;
        RegtimeCard.Text = jObject["data"]?["regtime"]?.ToString()!;
        QqCard.Text = jObject["data"]?["qq"]?.ToString()!;
        UsergroupCard.Text = jObject["data"]?["usergroup"]?.ToString()!;
        TermCard.Text = jObject["data"]?["term"]?.ToString()!;
        UserRealNameStatus.Text = jObject["data"]?["realname"]?.ToString()!;
        UserTunnelRestrictions.Text = $"{jObject["data"]?["tunnelCount"]} / {jObject["data"]?["tunnel"]}";
        var bandwidth = (int)jObject["data"]?["bandwidth"];
        UserBandwidthThrottling.Text = $"国内{bandwidth}m/国外{bandwidth * 4}m";
    }

    private void TokenClick(object sender, RoutedEventArgs e)
    {
        Token.Click -= TokenClick;
        _i++;

        switch (_i)
        {
            case 1:
                Reminders.Reminder_Box_Show(User.Usertoken);
                break;
            case 2:
                Clipboard.SetDataObject(User.Usertoken);
                Reminders.Reminder_Box_Show("Token已复制到的剪切板点击重新显示");
                _i = 0;
                break;
        }

        Token.Click += TokenClick;
    }
}