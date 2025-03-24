using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace ChmlFrp_Professional_Launcher
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
            Window window = GetWindow(this);
            window.Topmost = true;
            window.ShowInTaskbar = false;
            IntPtr hWnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
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

        private static readonly IntPtr HWND_TOPMOST = new(-1);
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;
    }
}
