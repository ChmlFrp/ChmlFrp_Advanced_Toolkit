namespace CPL.Pages.ChmlFrpPages;

public partial class ChmlFrpLoginPage
{
    public ChmlFrpLoginPage()
    {
        InitializeComponent();
        TextBoxPassword.Text = User.Password;
        TextBoxUsername.Text = User.Username;
    }

    private void Border_MouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e
    )
    {
        MainWindowClass.DragMove();
    }

    private async void Logon(object sender, RoutedEventArgs e)
    {
        LogonButton.Click -= Logon;

        if (!string.IsNullOrWhiteSpace(TextBoxUsername.Text)
            && !string.IsNullOrWhiteSpace(TextBoxPassword.Text))
        {
            if (TextBoxPassword.Text.Length is >= 6 and < 48)
            {
                if (await Downloadfiles.GetApItoLogin(true, TextBoxUsername.Text, TextBoxPassword.Text))
                {
                    await Task.Delay(1000);
                    Visibility = Visibility.Collapsed;
                    PagesClass.ChmlFrpHomePage = new ChmlFrpHomePage();
                    MainWindowClass.NavigateChmlfrpPage(null, null);

                    LogonButton.Click += Logon;
                }
            }
            else
            {
                Reminders.Reminder_Box_Show("密码不符合要求", "red");
            }
        }
        else
        {
            Reminders.Reminder_Box_Show("别输入空白字符", "red");
        }

        LogonButton.Click += Logon;
    }

    private void Exit(object sender, RoutedEventArgs e)
    {
        Visibility = Visibility.Collapsed;
        MainWindowClass.LaunchPageButton.IsChecked = true;
        MainWindowClass.ChmlfrpPageButton.Click += MainWindowClass.NavigateChmlfrpPage;
        MainWindowClass.PagesNavigation.Navigate(PagesClass.LaunchPage);
    }

    private async void Signup(object sender, RoutedEventArgs e)
    {
        Reminders.Reminder_Box_Show("跳转中...");
        await Task.Delay(500);
        Process.Start(new ProcessStartInfo("https://panel.chmlfrp.cn/sign"));
    }
}