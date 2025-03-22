using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChmlFrp_Professional_Launcher.Pages
{
    /// <summary>
    /// Reminder_Interface_Show.xaml 的交互逻辑
    /// </summary>
    public partial class Reminder_Interface_Show : Page
    {
        public Reminder_Interface_Show()
        {
            InitializeComponent();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow MainWindow = Application.Current.MainWindow as MainWindow;
            MainWindow.DragMove();
        }
    }
}
