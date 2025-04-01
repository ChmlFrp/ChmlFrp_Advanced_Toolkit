using System.Windows;
using System.Windows.Controls;
using ChmlFrp_Professional_Launcher.Pages.ChmlFrpPages.ChmlFrpLoginedPages;
using static ChmlFrp_Professional_Launcher.MainClass;

namespace ChmlFrp_Professional_Launcher.Pages.ChmlFrpPages
{
    /// <summary>
    /// ChmlFrpPage.xaml 的交互逻辑
    /// </summary>
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
}