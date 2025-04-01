using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;
using static ChmlFrp_Professional_Launcher.MainClass;

namespace ChmlFrp_Professional_Launcher.Pages.ChmlFrpPages.ChmlFrpLoginedPages
{
    /// <summary>
    /// HomePage.xaml 的交互逻辑
    /// </summary>
    public partial class HomePage
    {
        private readonly DispatcherTimer _timer;

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

        private int _count = 1;

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

            if (_count == 1)
            {
                _count = 0;
                var jsonContent1 = System.IO.File.ReadAllText(Paths.Temp.TempApiUser);
                var jsonObject1 = JObject.Parse(jsonContent1);
                await Downloadfiles.Downloadasync(
                    jsonObject1["data"]?["userimg"]?.ToString(),
                    Paths.Temp.TempUserImage
                );
            }

            var jsonContent = System.IO.File.ReadAllText(Paths.Temp.TempApiUser);
            var jsonObject = JObject.Parse(jsonContent);

            UserImage.ImageSource = new BitmapImage(new Uri(Paths.Temp.TempUserImage));
            UserTextBlock.Text = jsonObject["data"]?["username"]?.ToString();
            Usermailbox.Text = jsonObject["data"]?["email"]?.ToString();
            UserRegistration_time.Text = jsonObject["data"]?["regtime"]?.ToString();
            UserQQ.Text = jsonObject["data"]?["qq"]?.ToString();
            Userright.Text = jsonObject["data"]?["usergroup"]?.ToString();
            UserExpiration_time.Text = jsonObject["data"]?["term"]?.ToString();
            UserReal_name_status.Text = jsonObject["data"]?["realname"]?.ToString();
            UserPoints_remaining.Text = jsonObject["data"]?["integral"]?.ToString();

            UserTunnel_restrictions.Text =
                jsonObject["data"]?["tunnelCount"]?.ToString()
                + " / "
                + jsonObject["data"]?["tunnel"]?.ToString();

            UserBandwidth_throttling.Text =
                "国内" + jsonObject["data"]?["bandwidth"]?.ToString() + "m";
        }

        private int _i;

        private void TokenClick(object sender, RoutedEventArgs e)
        {
            Token.Click -= TokenClick;
            _i++;

            switch (_i)
            {
                case 1:
                    Reminders.Reminder_Box_Show(User.Usertoken, "green");
                    break;
                case 2:
                    Clipboard.SetDataObject(User.Usertoken);
                    Reminders.Reminder_Box_Show("Token已复制到的剪切板点击重新显示", "green");
                    Token.Content = "点击查看Token";
                    _i = 0;
                    break;
            }

            Token.Click += TokenClick;
        }
    }
}