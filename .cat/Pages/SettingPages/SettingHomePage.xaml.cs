namespace CPL.Pages.SettingPages;

public partial class SettingHomePage
{
    private readonly BlankPage _blankPage = new();

    public SettingHomePage()
    {
        InitializeComponent();
        ButtonBase_OnClick(null, null);
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        PagesNavigation.Navigate(_blankPage);
    }
}