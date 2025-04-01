using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq;
using static ChmlFrp_Professional_Launcher.MainClass;
using Microsoft.Win32;

namespace ChmlFrp_Professional_Launcher.Pages.ChmlFrpPages
{
    /// <summary>
    /// ChmlFrpLoginPage.xaml 的交互逻辑
    /// </summary>
    public partial class ChmlFrpLoginPage
    {
        public ChmlFrpLoginPage()
        {
            InitializeComponent();
            TextBox_password.Text = User.Password;
            TextBox_Username.Text = User.Username;
        }

        private void Border_MouseLeftButtonDown(
            object sender,
            System.Windows.Input.MouseButtonEventArgs e
        )
        {
            MainWindowClass.DragMove();
        }

        private async void Logon(object sender, RoutedEventArgs e)
        {
            logonButton.Click -= Logon;

            if (
                string.IsNullOrWhiteSpace(TextBox_Username.Text)
                || string.IsNullOrWhiteSpace(TextBox_password.Text)
            )
            {
                Reminders.Reminder_Box_Show("别输入空白字符", "red");
                logonButton.Click += Logon;
                return;
            }

            var registryKey = Registry.CurrentUser.OpenSubKey
                (@"SOFTWARE\\ChmlFrp", true);

            registryKey.SetValue("username", TextBox_Username.Text);
            registryKey.SetValue("password", TextBox_password.Text);

            if (Downloadfiles.GetApItoLogin(true))
            {
                var jsonContent = System.IO.File.ReadAllText(Paths.Temp.TempApiLogin);
                var jsonObject = JObject.Parse(jsonContent);
                registryKey.SetValue("usertoken", jsonObject["data"]?["usertoken"]?.ToString());
                new User();
                await Task.Delay(1000);
                Visibility = Visibility.Collapsed;
                PagesClass.ChmlFrpHomePage = new ChmlFrphomePage();
                MainWindowClass.NavigateChmlfrpPage(null, null);
            }

            logonButton.Click += Logon;
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            MainWindowClass.LaunchPageButton.IsChecked = true;
            MainWindowClass.ChmlfrpPageButton.Click += MainWindowClass.NavigateChmlfrpPage;
            MainWindowClass.PagesNavigation.Navigate(PagesClass.LaunchPage);
            return;
        }

        private async void Signup(object sender, RoutedEventArgs e)
        {
            Reminders.Reminder_Box_Show("跳转中...");
            await Task.Delay(500);
            Process.Start(new ProcessStartInfo("https://preview.panel.chmlfrp.cn/sign"));
        }
    }
}