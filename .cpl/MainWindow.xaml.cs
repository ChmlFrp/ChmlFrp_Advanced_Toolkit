using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using ChmlFrp_Professional_Launcher.Pages;
using ChmlFrp_Professional_Launcher.Pages.ChmlFrpPages;
using static ChmlFrp_Professional_Launcher.MainClass;

namespace ChmlFrp_Professional_Launcher
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public readonly BlankPage BlankPage = new();
        private static ChmlFrpLoginPage _chmlFrpLoginPage;

        public MainWindow()
        {
            MainWindowClass = this;
            InitializeComponent();

            StartWindow startWindow = new();
            startWindow.Show();
            Thread.Sleep(2000);
            startWindow.Close();

            if (!Downloadfiles.GetApItoLogin(false)) _chmlFrpLoginPage = new ChmlFrpLoginPage();
            Initialize.InitializeNext();
            NavigateLaunching(null, null);
        }

        public void NavigateChmlfrpPage(object sender, RoutedEventArgs e)
        {
            if (!SignInBool)
            {
                _chmlFrpLoginPage.Visibility = Visibility.Visible;
                RemindersNavigation.Navigate(_chmlFrpLoginPage);
            }

            PagesNavigation.Navigate(PagesClass.ChmlFrpHomePage);
        }

        private void NavigateLaunching(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(PagesClass.LaunchPage);
        }

        private void NavigateSettings(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(BlankPage);
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
}