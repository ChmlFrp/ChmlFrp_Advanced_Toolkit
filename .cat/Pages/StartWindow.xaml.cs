using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace CAT.Pages;

public partial class StartWindow
{
    private const uint SwpNomove = 0x0002;
    private const uint SwpNosize = 0x0001;

    private static readonly IntPtr HwndTopmost = new(-1);

    public StartWindow()
    {
        InitializeComponent();
        Loaded += StartWindow_Loaded;
    }

    private async void StartWindow_Loaded(object sender, RoutedEventArgs e)
    {
        var window = GetWindow(this);
        if (window == null) return;
        window.Topmost = true;
        window.ShowInTaskbar = false;
        var hWnd = new WindowInteropHelper(window).Handle;
        SetWindowPos(hWnd, HwndTopmost, 0, 0, 0, 0, SwpNomove | SwpNosize);

        Initialize.InitializeFirst();
        if (!await Downloadfiles.GetApItoLogin(false))
            MainWindowClass.ChmlFrpLoginPage = new ChmlFrpLoginPage();
        PagesClass.ChmlFrpHomePage = new();
        PagesClass.LaunchPage = new();
        Initialize.InitializeNext();
        MainWindowClass.NavigateLaunching(null, null);

        await Task.Delay(1000);
        Close();
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int x,
        int y,
        int cx,
        int cy,
        uint uFlags
    );
}