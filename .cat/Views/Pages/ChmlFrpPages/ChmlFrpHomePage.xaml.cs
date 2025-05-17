namespace CAT.Views.Pages.ChmlFrpPages;

public partial class ChmlFrpHomePage
{
    private readonly UserinfoPage _homePage;
    private readonly TunnelPage _tmaPage;

    public ChmlFrpHomePage()
    {
        InitializeComponent();
        if (!User.SignInBool)
            return;
        _homePage = new UserinfoPage();
        _tmaPage = new TunnelPage();
        rdLaunchPage_Click(null, null);
    }

    private void rdLaunchPage_Click(object sender, RoutedEventArgs e)
    {
        PagesNavigation.Navigate(_homePage);
    }

    private void rdTMA_Click(object sender, RoutedEventArgs e)
    {
        PagesNavigation.Navigate(_tmaPage);
    }
}