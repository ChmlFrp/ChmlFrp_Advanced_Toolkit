using System.Windows;
using System.Windows.Controls;
using ChmlFrp_Professional_Launcher.Pages.ChmlFrpLoginPages;
using static ChmlFrp_Professional_Launcher.MainClass;

namespace ChmlFrp_Professional_Launcher.Pages
{
    /// <summary>
    /// ChmlfrpPage.xaml 的交互逻辑
    /// </summary>
    public partial class ChmlFrphomePage : Page
    {
        private HomePage HomePage;
        private TMAPage TMAPage;

        public ChmlFrphomePage()
        {
            InitializeComponent();
            if (!SignInBool)
                return;
            HomePage = new();
            TMAPage = new();
            rdLaunchPage_Click(null, null);
        }

        public void rdLaunchPage_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(HomePage);
        }

        private void rdTMA_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(TMAPage);
        }
    }
}
