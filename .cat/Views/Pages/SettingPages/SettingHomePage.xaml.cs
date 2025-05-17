namespace CAT.Views.Pages.SettingPages;

public partial class SettingHomePage
{
    private readonly AboutMePage _aboutMePage = new();
    private readonly PersonalizationPage _personalizationPage = new();

    public SettingHomePage()
    {
        InitializeComponent();
        NavigatePersonalizationPageClick(null, null);
    }

    private void NavigatePersonalizationPageClick(object sender, RoutedEventArgs e)
    {
        PagesNavigation.Navigate(_personalizationPage);
    }

    private void NavigateAboutMePageClick(object sender, RoutedEventArgs e)
    {
        PagesNavigation.Navigate(_aboutMePage);
    }
}