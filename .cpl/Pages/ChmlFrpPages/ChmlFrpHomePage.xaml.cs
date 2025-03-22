using ChmlFrp_Professional_Launcher.Pages.ChmlFrpLoginPages;
using System.Windows;
using System.Windows.Controls;

namespace ChmlFrp_Professional_Launcher.Pages
{
    /// <summary>
    /// ChmlfrpPage.xaml 的交互逻辑
    /// </summary>
    public partial class ChmlFrphomePage : Page
    {
        MainWindow MainWindow = Application.Current.MainWindow as MainWindow;
        HomePage HomePage;
        TMAPage TMAPage;

        public ChmlFrphomePage()
        {
            InitializeComponent();
            if (MainWindow.SignInBool)
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
