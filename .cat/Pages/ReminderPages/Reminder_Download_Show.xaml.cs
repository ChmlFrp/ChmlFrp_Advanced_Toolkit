namespace CAT.Pages.ReminderPages;

public partial class ReminderDownloadShow
{
    private int _i;

    public ReminderDownloadShow()
    {
        InitializeComponent();
    }

    private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        MainWindowClass.DragMove();
    }

    private async void Download_Button_Click(object sender, RoutedEventArgs e)
    {
        if (_i == 1)
        {
            Reminders.Reminder_Box_Show("请勿重复点击", "red");
            return;
        }

        var isX86Checked = X86Butten.IsChecked == true;
        var isAmdChecked = AmdButten.IsChecked == true;

        if (!isX86Checked && !isAmdChecked)
        {
            Reminders.Reminder_Box_Show("选项未选择", "red");
            return;
        }

        Reminders.Reminder_Box_Show("正在下载中...");
        PorgressBar.IsIndeterminate = true;
        _i++;
        var downloadSuccess = await Task.Run(async () =>
        {
            if (isX86Checked)
                return await Downloadfiles.Downloadasync(
                    "http://cat.chmlfrp.com/frp/frpc_86.exe",
                    Paths.FrpExePath
                );

            if (isAmdChecked)
                return await Downloadfiles.Downloadasync(
                    "http://cat.chmlfrp.com/frp/frpc_amd.exe",
                    Paths.FrpExePath
                );

            return false;
        });

        if (downloadSuccess)
        {
            Reminders.Reminder_Box_Show("下载成功");
        }
        else
        {
            Reminders.Reminder_Box_Show("下载失败", "red");
            PorgressBar.IsIndeterminate = false;
            _i--;
            return;
        }

        await Task.Delay(1000);
        Visibility = Visibility.Collapsed;
    }
}