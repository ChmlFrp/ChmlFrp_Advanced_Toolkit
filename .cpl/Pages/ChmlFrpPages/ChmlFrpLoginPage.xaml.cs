using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using IniParser;
using IniParser.Model;
using Newtonsoft.Json.Linq;

namespace ChmlFrp_Professional_Launcher.Pages
{
    /// <summary>
    /// ChmlFrpLoginPage.xaml 的交互逻辑
    /// </summary>
    public partial class ChmlFrpLoginPage : Page
    {
        MainWindow MainWindow = Application.Current.MainWindow as MainWindow;
        private IniData data;
        private FileIniDataParser parser = new();

        public ChmlFrpLoginPage()
        {
            InitializeComponent();
            data = parser.ReadFile(MainClass.Paths.setupIniPath);
        }

        private void Border_MouseLeftButtonDown(
            object sender,
            System.Windows.Input.MouseButtonEventArgs e
        )
        {
            MainWindow.DragMove();
        }

        private async void logon(object sender, RoutedEventArgs e)
        {
            logonButton.Click -= logon;

            if (
                string.IsNullOrWhiteSpace(TextBox_Username.Text)
                || string.IsNullOrWhiteSpace(TextBox_password.Text)
            )
            {
                MainClass.Reminders.Reminder_Box_Show("别输入空白字符", "red");
                logonButton.Click += logon;
                return;
            }

            data["ChmlFrp_Professional_Launcher Setup"]["Password"] = TextBox_password.Text;
            data["ChmlFrp_Professional_Launcher Setup"]["Username"] = TextBox_Username.Text;
            parser.WriteFile(MainClass.Paths.setupIniPath, data);
            if (MainClass.Downloadfiles.GetAPItoLogin(true))
            {
                MainWindow.SignInBool = false;

                string jsonContent = System.IO.File.ReadAllText(
                    MainClass.Paths.Temp.temp_api_login
                );
                var jsonObject = JObject.Parse(jsonContent);
                data["ChmlFrp_Professional_Launcher Setup"]["Token"] = jsonObject["data"]
                    ["usertoken"]
                    ?.ToString();
                parser.WriteFile(MainClass.Paths.setupIniPath, data);

                await Task.Delay(1000);

                this.Visibility = Visibility.Collapsed;

                MainWindow.ChmlFrpHomePage = new();
                MainWindow.PagesNavigation.Navigate(MainWindow.ChmlFrpHomePage);
                return;
            }
            else
            {
                logonButton.Click += logon;
                return;
            }
        }

        private void exit(object sender, RoutedEventArgs e)
        {
            MainWindow.ChmlFrpLoginPage.Visibility = Visibility.Collapsed;
            MainWindow.LaunchPageButton.IsChecked = true;
            MainWindow.ChmlfrpPageButton.Click += MainWindow.rdChmlfrpPage_Click;
            MainWindow.PagesNavigation.Navigate(MainWindow.LaunchPage);
            return;
        }

        private async void signup(object sender, RoutedEventArgs e)
        {
            MainClass.Reminders.Reminder_Box_Show("跳转中...");
            await Task.Delay(500);
            Process.Start(new ProcessStartInfo("https://preview.panel.chmlfrp.cn/sign"));
        }
    }
}
