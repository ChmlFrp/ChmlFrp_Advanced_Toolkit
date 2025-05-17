using System.Diagnostics;
using System.Reflection;

namespace CAT.Views.Pages.SettingPages;

public partial class AboutMePage
{
    public AboutMePage()
    {
        InitializeComponent();
    }

    private async void Github_Click(object sender, RoutedEventArgs e)
    {
        Reminders.Reminder_Box_Show("跳转中...");
        await Task.Delay(500);
        Process.Start(new ProcessStartInfo("https://cat.chmlfrp.com"));
    }

    private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        Reminders.Reminder_Box_Show("加载中...");
        await Task.Delay(500);
        Reminders.Reminder_Interface_Show("软件版本信息显示",
            $"次版本号：{Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version}\n大版本号：{Assembly.GetExecutingAssembly().GetName().Version}\n版权：{Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright}");
    }
}