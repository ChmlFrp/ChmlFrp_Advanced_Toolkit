using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static ChmlFrp_Professional_Launcher.MainClass;

namespace ChmlFrp_Professional_Launcher.Pages.ReminderPages
{
    /// <summary>
    /// Reminder_Download_Show.xaml 的交互逻辑
    /// </summary>
    public partial class ReminderDownloadShow
    {
        public ReminderDownloadShow()
        {
            InitializeComponent();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainWindowClass.DragMove();
        }

        private int i;

        private async void Download_Button_Click(object sender, RoutedEventArgs e)
        {
            if (i == 1)
            {
                Reminders.Reminder_Box_Show("请勿重复点击", "red");
                return;
            }

            var isX86Checked = X86_Butten.IsChecked == true;
            var isAmdChecked = AMD_Butten.IsChecked == true;

            if (!isX86Checked && !isAmdChecked)
            {
                Reminders.Reminder_Box_Show("选项未选择", "red");
                return;
            }

            Reminders.Reminder_Box_Show("正在下载中...", "green");
            PorgressBar.IsIndeterminate = true;
            i++;
            var downloadSuccess = await Task.Run(async () =>
            {
                if (isX86Checked)
                    return await Downloadfiles.Downloadasync(
                        "https://cpl.chmlfrp.com/frp/frpc_86.exe",
                        Paths.FrpExePath
                    );

                if (isAmdChecked)
                    return await Downloadfiles.Downloadasync(
                        "https://cpl.chmlfrp.com/frp/frpc_amd.exe",
                        Paths.FrpExePath
                    );

                return false;
            });

            if (downloadSuccess)
            {
                Reminders.Reminder_Box_Show("下载成功", "green");
            }
            else
            {
                Reminders.Reminder_Box_Show("下载失败", "red");
                i--;
                return;
            }

            await Task.Delay(1000);
            Visibility = Visibility.Collapsed;
        }
    }
}