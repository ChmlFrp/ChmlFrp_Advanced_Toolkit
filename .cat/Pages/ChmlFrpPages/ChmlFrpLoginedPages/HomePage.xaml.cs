namespace CPL.Pages.ChmlFrpPages.ChmlFrpLoginedPages;

public partial class HomePage
{
    private readonly DispatcherTimer _timer;

    private int _i;

    public HomePage()
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
        if (
            !await Downloadfiles.Downloadasync(
                $"https://cf-v2.uapis.cn/userinfo?token={User.Usertoken}",
                Paths.Temp.TempApiUser
            )
        )
        {
            Reminders.Reminder_Box_Show("用户信息加载失败", "red");
            return;
        }

        var jsonObject = JObject.Parse(File.ReadAllText(Paths.Temp.TempApiUser));

        Paths.Temp.TempUserImage = Path.GetTempFileName();

        await Downloadfiles.Downloadasync(
            jsonObject["data"]?["userimg"]?.ToString(),
            Paths.Temp.TempUserImage
        );

        UserImage.ImageSource = new BitmapImage(new Uri(Paths.Temp.TempUserImage));
        UserTextBlock.Text = jsonObject["data"]?["username"]?.ToString();
        Usermailbox.Text = jsonObject["data"]?["email"]?.ToString();
        UserRegistrationTime.Text = jsonObject["data"]?["regtime"]?.ToString();
        UserQq.Text = jsonObject["data"]?["qq"]?.ToString();
        Userright.Text = jsonObject["data"]?["usergroup"]?.ToString();
        UserExpirationTime.Text = jsonObject["data"]?["term"]?.ToString();
        UserRealNameStatus.Text = jsonObject["data"]?["realname"]?.ToString();
        UserPointsRemaining.Text = jsonObject["data"]?["integral"]?.ToString();

        UserTunnelRestrictions.Text =
            jsonObject["data"]?["tunnelCount"]
            + " / "
            + jsonObject["data"]?["tunnel"];

        UserBandwidthThrottling.Text =
            "国内" + jsonObject["data"]?["bandwidth"] + "m";
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
                Token.Content = "点击查看Token";
                _i = 0;
                break;
        }

        Token.Click += TokenClick;
    }
}