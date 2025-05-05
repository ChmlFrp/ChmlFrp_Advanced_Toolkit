namespace CAT;

public partial class MainWindow
{
    public ChmlFrpLoginPage ChmlFrpLoginPage;
    private readonly Pages.SettingPages.SettingHomePage _settingHomePage = new();

    public MainWindow()
    {
        MainWindowClass = this;
        InitializeComponent();
        InitializeAsync();
    }

    private void InitializeAsync()
    {
        StartWindow startWindow = new();
        startWindow.ShowDialog();
        Visibility = Visibility.Visible;
    }

    public void NavigateChmlfrpPage(object sender, RoutedEventArgs e)
    {
        if (!SignInBool)
        {
            ChmlFrpLoginPage.Visibility = Visibility.Visible;
            RemindersNavigation.Navigate(ChmlFrpLoginPage);
        }

        PagesNavigation.Navigate(PagesClass.ChmlFrpHomePage);
    }

    public void NavigateLaunching(object sender, RoutedEventArgs e)
    {
        PagesNavigation.Navigate(PagesClass.LaunchPage);
    }

    private void NavigateSettings(object sender, RoutedEventArgs e)
    {
        PagesNavigation.Navigate(_settingHomePage);
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