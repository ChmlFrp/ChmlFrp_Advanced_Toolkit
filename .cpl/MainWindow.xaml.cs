using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ChmlFrp_Professional_Launcher.Pages;
using static ChmlFrp_Professional_Launcher.MainClass;

namespace ChmlFrp_Professional_Launcher
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private BlankPage BlankPage = new();
        public static ChmlFrpLoginPage ChmlFrpLoginPage;

        public MainWindow()
        {
            MainWindowClass = this;
            Initialize();
            InitializeComponent();
            StartWindow StartWindow = new();
            StartWindow.Show();
            Thread.Sleep(2000);
            StartWindow.Close();

            if (!Downloadfiles.GetAPItoLogin(false))
                ChmlFrpLoginPage = new();

            string[] imageFiles = Directory
                .GetFiles(Paths.pictures_path, "*.*")
                .Where(file =>
                    file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                    || file.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                )
                .ToArray();

            if (imageFiles.Length > 0)
            {
                Random random = new();
                string randomImage = imageFiles[random.Next(imageFiles.Length)];
                Imagewallpaper.ImageSource = new BitmapImage(
                    new Uri(randomImage, UriKind.RelativeOrAbsolute)
                );
                Imagewallpaper.Stretch = Stretch.UniformToFill;
            }

            rdLaunchPage_Click(null, null);
            Update();
            if (!File.Exists(Paths.frpExePath))
            {
                Reminders.Reminder_Download_Show();
            }
        }

        public void rdChmlfrpPage_Click(object sender, RoutedEventArgs e)
        {
            if (!SignInBool)
            {
                ChmlFrpLoginPage.Visibility = Visibility.Visible;
                RemindersNavigation.Navigate(ChmlFrpLoginPage);
            }

            PagesNavigation.Navigate(PagesClass.ChmlFrpHomePage);
        }

        private void rdLaunchPage_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(PagesClass.LaunchPage);
        }

        private void rdSettingsPage_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(BlankPage);
        }

        protected override void OnClosed(EventArgs e)
        {
            Reminders.LogsOutputting("退出软件中...");
            base.OnClosed(e);
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
