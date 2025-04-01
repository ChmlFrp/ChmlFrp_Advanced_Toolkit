using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace ChmlFrp_Professional_Launcher.Pages
{
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
            Loaded += StartWindow_Loaded;
        }

        private void StartWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var window = GetWindow(this);
            window.Topmost = true;
            window.ShowInTaskbar = false;
            var hWnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            SetWindowPos(hWnd, HwndTopmost, 0, 0, 0, 0, SwpNomove | SwpNosize);
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

        private static readonly IntPtr HwndTopmost = new(-1);
        private const uint SwpNomove = 0x0002;
        private const uint SwpNosize = 0x0001;
    }
}