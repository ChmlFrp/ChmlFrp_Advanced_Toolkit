using System.Threading;

namespace CPL;

public partial class MainWindow
{
    private static ChmlFrpLoginPage _chmlFrpLoginPage;
    public readonly Pages.SettingPages.SettingHomePage SettingHomePage = new();

    public MainWindow()
    {
        MainWindowClass = this;
        Initialize.InitializeFirst();
        InitializeComponent();
        StartWindow startWindow = new();
        startWindow.Show();
        Thread.Sleep(2000);
        startWindow.Close();

        if (!Downloadfiles.GetApItoLogin(false)) _chmlFrpLoginPage = new ChmlFrpLoginPage();
        PagesClass.ChmlFrpHomePage = new();
        Initialize.InitializeNext();
        NavigateLaunching(null, null);
    }

    public void NavigateChmlfrpPage(object sender, RoutedEventArgs e)
    {
        if (!SignInBool)
        {
            _chmlFrpLoginPage.Visibility = Visibility.Visible;
            RemindersNavigation.Navigate(_chmlFrpLoginPage);
        }

        PagesNavigation.Navigate(PagesClass.ChmlFrpHomePage);
    }

    private void NavigateLaunching(object sender, RoutedEventArgs e)
    {
        PagesNavigation.Navigate(PagesClass.LaunchPage);
    }

    private void NavigateSettings(object sender, RoutedEventArgs e)
    {
        PagesNavigation.Navigate(SettingHomePage);
    }

    protected override void OnClosed(EventArgs e)
    {
        Reminders.LogsOutputting("退出软件中...");
        base.OnClosed(e);
    }

    private void MinimizeThis(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void DragMoveThis(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void CloseThis(object sender, RoutedEventArgs e)
    {
        Close();
    }
}