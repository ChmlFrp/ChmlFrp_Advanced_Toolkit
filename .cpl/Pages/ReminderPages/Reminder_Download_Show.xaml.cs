using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChmlFrp_Professional_Launcher.Pages
{
    /// <summary>
    /// Reminder_Download_Show.xaml 的交互逻辑
    /// </summary>
    public partial class Reminder_Download_Show : Page
    {
        MainWindow MainWindow = Application.Current.MainWindow as MainWindow;

        public Reminder_Download_Show()
        {
            InitializeComponent();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.DragMove();
        }

        int i;

        private async void Download_Button_Click(object sender, RoutedEventArgs e)
        {
            if (i == 1)
            {
                MainClass.Reminders.Reminder_Box_Show("请勿重复点击", "red");
                return;
            }

            bool isX86Checked = X86_Butten.IsChecked == true;
            bool isAMDChecked = AMD_Butten.IsChecked == true;

            if (!isX86Checked && !isAMDChecked)
            {
                MainClass.Reminders.Reminder_Box_Show("选项未选择", "red");
                return;
            }

            MainClass.Reminders.Reminder_Box_Show("正在下载中...", "green");
            PorgressBar.IsIndeterminate = true;
            i++;
            bool downloadSuccess = await Task.Run(async () =>
            {
                if (isX86Checked)
                    return await MainClass.Downloadfiles.Downloadasync(
                        "https://cpl.chmlfrp.com/frp/frpc_86.exe",
                        MainClass.Paths.frpExePath
                    );

                if (isAMDChecked)
                    return await MainClass.Downloadfiles.Downloadasync(
                        "https://cpl.chmlfrp.com/frp/frpc_amd.exe",
                        MainClass.Paths.frpExePath
                    );

                return false;
            });

            if (downloadSuccess)
            {
                MainClass.Reminders.Reminder_Box_Show("下载成功", "green");
            }
            else
            {
                MainClass.Reminders.Reminder_Box_Show("下载失败", "red");
                i--;
                return;
            }

            await Task.Delay(1000);
            Visibility = Visibility.Collapsed;
        }
    }
}
