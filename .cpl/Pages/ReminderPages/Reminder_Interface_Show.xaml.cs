namespace CPL.Pages.ReminderPages;

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

    public void Close(object sender, RoutedEventArgs e)
    {
        Visibility = Visibility.Collapsed;
    }

    public async void Update(object sender, RoutedEventArgs e)
    {
        Visibility = Visibility.Collapsed;

        var jObject = JObject.Parse(File.ReadAllText(Paths.Temp.TempApiLogin));

        await Task.Delay(1000);

        var exe = Path.Combine(
            Paths.CplPath,
            "CPL.exe"
        );

        if (Downloadfiles.Download(jObject["url"]?.ToString(), exe))
        {
            Reminders.Reminder_Box_Show("下载成功");
            Reminders.LogsOutputting("下载成功");

            var batchFilePath = Path.Combine(Paths.CplPath, "update.bat");
            var processPath = Process.GetCurrentProcess().MainModule?.FileName;
            var batchContent =
                $@"
                    @echo off
                    timeout /t 3 /nobreak
                    move /y ""{exe}"" ""{processPath}""
                    start """" ""{processPath}""
                    exit
                    ";

            File.WriteAllText(batchFilePath, batchContent);

            var process = new Process();
            ProcessStartInfo processInfo = new(
                "cmd.exe",
                $"/c start {batchFilePath}"
            )
            {
                UseShellExecute = true,
                CreateNoWindow = true
            };
            process.StartInfo = processInfo;
            process.Start();

            MainWindowClass.Close();
        }
        else
        {
            Reminders.Reminder_Box_Show("更新失败", "red");
            Reminders.LogsOutputting("更新失败");
            Visibility = Visibility.Collapsed;
        }
    }
}