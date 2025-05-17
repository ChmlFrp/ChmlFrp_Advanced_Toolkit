using CAT.Views.Pages.ChmlFrpPages;

namespace CAT.Views.Windows.Views.Windows;

public partial class StartWindow
{
    public StartWindow()
    {
        InitializeComponent();
        Loaded += StartWindow_Loaded;
    }

    private async void StartWindow_Loaded(object sender, RoutedEventArgs e)
    {
        Initialize.InitializeFirst();
        if (!await Api.GetApItoLogin(false))
            ViewsClass.MainWindowClass.ChmlFrpLoginPage = new LogininPage();
        ViewsClass.ChmlFrpHomePage = new();
        ViewsClass.LaunchPage = new();
        ViewsClass.SettingHomePage = new();
        Initialize.InitializeNext();
        ViewsClass.MainWindowClass.NavigateLaunching(null, null);
        Close();
    }
}