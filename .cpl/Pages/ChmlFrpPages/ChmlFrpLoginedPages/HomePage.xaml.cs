using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using IniParser;
using IniParser.Model;
using Newtonsoft.Json.Linq;

namespace ChmlFrp_Professional_Launcher.Pages.ChmlFrpLoginPages
{
    /// <summary>
    /// HomePage.xaml 的交互逻辑
    /// </summary>
    public partial class HomePage : Page
    {
        private FileIniDataParser parser;
        private IniData data;

        private DispatcherTimer timer;

        public HomePage()
        {
            InitializeComponent();
            new MainClass.User();
            parser = new FileIniDataParser();
            data = parser.ReadFile(MainClass.Paths.setupIniPath);

            InitializeApps();

            // 设置定时器
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(30);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            InitializeApps();
        }

        private async void InitializeApps()
        {
            if (
                !await MainClass.Downloadfiles.Downloadasync(
                    "http://cf-v2.uapis.cn/userinfo?token="
                        + data["ChmlFrp_Professional_Launcher Setup"]["Token"],
                    MainClass.Paths.Temp.temp_api_user
                )
            )
                MainClass.Reminders.Reminder_Box_Show("用户信息加载失败", "red");

            string jsonContent1 = System.IO.File.ReadAllText(MainClass.Paths.Temp.temp_api_user);
            var jsonObject1 = JObject.Parse(jsonContent1);
            await MainClass.Downloadfiles.Downloadasync(
                jsonObject1["data"]["userimg"]?.ToString(),
                MainClass.Paths.Temp.temp_user_image
            );

            string jsonContent = System.IO.File.ReadAllText(MainClass.Paths.Temp.temp_api_user);
            var jsonObject = JObject.Parse(jsonContent);

            UserImage.ImageSource = new BitmapImage(new Uri(MainClass.Paths.Temp.temp_user_image));
            UserTextBlock.Text = jsonObject["data"]["username"]?.ToString();
            Usermailbox.Text = jsonObject["data"]["email"]?.ToString();
            UserRegistration_time.Text = jsonObject["data"]["regtime"]?.ToString();
            UserQQ.Text = jsonObject["data"]["qq"]?.ToString();
            Userright.Text = jsonObject["data"]["usergroup"]?.ToString();
            UserExpiration_time.Text = jsonObject["data"]["term"]?.ToString();
            UserReal_name_status.Text = jsonObject["data"]["realname"]?.ToString();
            UserPoints_remaining.Text = jsonObject["data"]["integral"]?.ToString();

            UserTunnel_restrictions.Text =
                jsonObject["data"]["tunnelCount"]?.ToString()
                + " / "
                + jsonObject["data"]["tunnel"]?.ToString();

            UserBandwidth_throttling.Text =
                "国内" + jsonObject["data"]["bandwidth"]?.ToString() + "m";
        }

        int i;

        private void TokenClick(object sender, RoutedEventArgs e)
        {
            Token.Click -= TokenClick;
            i++;

            if (i == 1)
                MainClass.Reminders.Reminder_Box_Show(MainClass.User.usertoken, "green");
            if (i == 2)
            {
                Clipboard.SetDataObject(MainClass.User.usertoken);
                MainClass.Reminders.Reminder_Box_Show("Token已复制到的剪切板点击重新显示", "green");
                Token.Content = "点击查看Token";
                i = 0;
            }
            Token.Click += TokenClick;
        }
    }
}
