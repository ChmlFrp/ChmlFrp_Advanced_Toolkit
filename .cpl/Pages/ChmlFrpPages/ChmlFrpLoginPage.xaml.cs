using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using IniParser;
using IniParser.Model;
using Newtonsoft.Json.Linq;
using static ChmlFrp_Professional_Launcher.MainClass;

namespace ChmlFrp_Professional_Launcher.Pages
{
    /// <summary>
    /// ChmlFrpLoginPage.xaml 的交互逻辑
    /// </summary>
    public partial class ChmlFrpLoginPage : Page
    {
        private IniData data;
        private FileIniDataParser parser = new();

        public ChmlFrpLoginPage()
        {
            InitializeComponent();
            data = parser.ReadFile(Paths.setupIniPath);

            TextBox_password.Text = User.password;
            TextBox_Username.Text = User.username;
        }

        private void Border_MouseLeftButtonDown(
            object sender,
            System.Windows.Input.MouseButtonEventArgs e
        )
        {
            MainWindowClass.DragMove();
        }

        private async void logon(object sender, RoutedEventArgs e)
        {
            logonButton.Click -= logon;

            if (
                string.IsNullOrWhiteSpace(TextBox_Username.Text)
                || string.IsNullOrWhiteSpace(TextBox_password.Text)
            )
            {
                Reminders.Reminder_Box_Show("别输入空白字符", "red");
                logonButton.Click += logon;
                return;
            }

            data["ChmlFrp_Professional_Launcher Setup"]["Password"] = TextBox_password.Text;
            data["ChmlFrp_Professional_Launcher Setup"]["Username"] = TextBox_Username.Text;
            parser.WriteFile(Paths.setupIniPath, data);

            if (Downloadfiles.GetAPItoLogin(true))
            {
                string jsonContent = System.IO.File.ReadAllText(Paths.Temp.temp_api_login);
                var jsonObject = JObject.Parse(jsonContent);
                data["ChmlFrp_Professional_Launcher Setup"]["Token"] = jsonObject["data"]
                    ["usertoken"]
                    ?.ToString();
                parser.WriteFile(Paths.setupIniPath, data);

                await Task.Delay(1000);

                this.Visibility = Visibility.Collapsed;
                PagesClass.ChmlFrpHomePage = new();
                MainWindowClass.rdChmlfrpPage_Click(null, null);
            }

            logonButton.Click += logon;
        }

        private void exit(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            MainWindowClass.LaunchPageButton.IsChecked = true;
            MainWindowClass.ChmlfrpPageButton.Click += MainWindowClass.rdChmlfrpPage_Click;
            MainWindowClass.PagesNavigation.Navigate(PagesClass.LaunchPage);
            return;
        }

        private async void signup(object sender, RoutedEventArgs e)
        {
            Reminders.Reminder_Box_Show("跳转中...");
            await Task.Delay(500);
            Process.Start(new ProcessStartInfo("https://preview.panel.chmlfrp.cn/sign"));
        }
    }
}
