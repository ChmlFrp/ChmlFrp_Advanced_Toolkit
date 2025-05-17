using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Interop;
using CAT.Views.Pages.ChmlFrpPages;
using CAT.Views.Windows.Views.Windows;

namespace CAT.Views.Windows;

public partial class MainWindow
{
    public LogininPage ChmlFrpLoginPage;

    public MainWindow()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is not Exception ex) return;
            Reminders.LogsOutputting($"异常退出软件: \n{ex.Message}");
            Process.Start(new ProcessStartInfo
            {
                FileName = Paths.LogfilePath,
                UseShellExecute = true
            });
        };

        ViewsClass.MainWindowClass = this;
        InitializeComponent();
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        StartWindow startWindow = new();
        startWindow.ShowDialog();
        Visibility = Visibility.Visible;
        Topmost = true;
        await Task.Delay(1000);
        Topmost = false;
    }

    public void NavigateChmlfrpPage(object sender, RoutedEventArgs e)
    {
        if (!User.SignInBool)
        {
            ChmlFrpLoginPage.Visibility = Visibility.Visible;
            RemindersNavigation.Navigate(ChmlFrpLoginPage);
        }

        PagesNavigation.Navigate(ViewsClass.ChmlFrpHomePage);
    }

    public void NavigateLaunching(object sender, RoutedEventArgs e)
    {
        PagesNavigation.Navigate(ViewsClass.LaunchPage);
    }

    private void NavigateSettings(object sender, RoutedEventArgs e)
    {
        PagesNavigation.Navigate(ViewsClass.SettingHomePage);
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

    protected override void OnClosed(EventArgs e)
    {
        Reminders.LogsOutputting("软件退出正常");
        base.OnClosed(e);
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        var hwndSource = (HwndSource)PresentationSource.FromVisual(this);
        hwndSource?.AddHook((IntPtr _, int msg, IntPtr _, IntPtr lParam, ref bool handled) =>
        {
            const int wmNchittest = 0x0084;
            const int resizeBorder = 4;
            if (msg != wmNchittest) return IntPtr.Zero;
            var pos = PointFromScreen(new Point(lParam.ToInt32() & 0xFFFF, lParam.ToInt32() >> 16));
            var w = ActualWidth;
            var h = ActualHeight;
            if (pos.Y <= resizeBorder)
            {
                if (pos.X <= resizeBorder)
                {
                    handled = true;
                    return (IntPtr)13;
                }

                if (pos.X >= w - resizeBorder)
                {
                    handled = true;
                    return (IntPtr)14;
                }

                handled = true;
                return (IntPtr)12;
            }

            if (pos.Y >= h - resizeBorder)
            {
                if (pos.X <= resizeBorder)
                {
                    handled = true;
                    return (IntPtr)16;
                }

                if (pos.X >= w - resizeBorder)
                {
                    handled = true;
                    return (IntPtr)17;
                }

                handled = true;
                return (IntPtr)15;
            }

            if (pos.X <= resizeBorder)
            {
                handled = true;
                return (IntPtr)10;
            }

            if (!(pos.X >= w - resizeBorder)) return IntPtr.Zero;
            handled = true;
            return (IntPtr)11;
        });
    }
}