using ChmlFrp_Advanced_Toolkit.Pages.SettingPages;

namespace CAT.Pages.SettingPages;

public partial class SettingHomePage
{
    private readonly PersonalizationPage _personalizationPage = new();
    private readonly AboutMePage _aboutMePage = new();

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