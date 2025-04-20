namespace CPL.Pages.ChmlFrpPages;

public partial class ChmlFrphomePage
{
    private readonly HomePage _homePage;
    private readonly TmaPage _tmaPage;

    public ChmlFrphomePage()
    {
        InitializeComponent();
        if (!SignInBool)
            return;
        _homePage = new HomePage();
        _tmaPage = new TmaPage();
        rdLaunchPage_Click(null, null);
    }

    public void rdLaunchPage_Click(object sender, RoutedEventArgs e)
    {
        PagesNavigation.Navigate(_homePage);
    }

    private void rdTMA_Click(object sender, RoutedEventArgs e)
    {
        PagesNavigation.Navigate(_tmaPage);
    }
}