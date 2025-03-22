using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ChmlFrp_Professional_Launcher.Pages;

namespace ChmlFrp_Professional_Launcher
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        BlankPage BlankPage;
        public LaunchPage LaunchPage;
        public ChmlFrphomePage ChmlFrpHomePage;
        public ChmlFrpLoginPage ChmlFrpLoginPage;

        public bool SignInBool;

        public MainWindow()
        {
            // 弹出加载页
            StartWindow StartWindow = new();
            StartWindow.Show();
            Thread.Sleep(2000);
            StartWindow.Close();

            if (!MainClass.Downloadfiles.GetAPItoLogin(false))
                SignInBool = true;
            else
                SignInBool = false;
            // 初始化页面
            LaunchPage = new();
            BlankPage = new();
            ChmlFrpHomePage = new();
            // 初始化主窗口
            InitializeComponent();

            // 显示背景图片
            string[] imageFiles = Directory
                .GetFiles(MainClass.Paths.pictures_path, "*.*")
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
            MainClass.Reminders.LogsOutputting("背景图片或默认加载成功");
            // 进入启动页
            rdLaunchPage_Click(this, new RoutedEventArgs());
            if (!File.Exists(MainClass.Paths.frpExePath))
            {
                MainClass.Reminders.Reminder_Download_Show();
            }
            MainClass.Update();
        }

        public void rdChmlfrpPage_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(ChmlFrpHomePage);
            if (SignInBool)
            {
                ChmlFrpLoginPage = new();
                RemindersNavigation.Navigate(ChmlFrpLoginPage);
            }
        }

        private void rdLaunchPage_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(LaunchPage);
        }

        private void rdSettingsPage_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(BlankPage);
        }

        protected override void OnClosed(EventArgs e)
        {
            MainClass.Reminders.LogsOutputting("退出软件中...");
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
