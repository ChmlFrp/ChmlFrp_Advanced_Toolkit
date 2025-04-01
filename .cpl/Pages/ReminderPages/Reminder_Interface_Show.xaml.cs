using System.Windows.Input;
using static ChmlFrp_Professional_Launcher.MainClass;

namespace ChmlFrp_Professional_Launcher.Pages.ReminderPages
{
    /// <summary>
    /// Reminder_Interface_Show.xaml 的交互逻辑
    /// </summary>
    public partial class ReminderInterfaceShow
    {
        public ReminderInterfaceShow()
        {
            InitializeComponent();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainWindowClass.DragMove();
        }
    }
}