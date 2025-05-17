using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CAT.Views.Windows.Classes;

public static class Initialize
{
    private static Mutex _mutex;

    public static void InitializeNext()
    {
        IsUpdate();
        SetImage();
        if (!File.Exists(Paths.FrpExePath)) Reminders.Reminder_Download_Show();
    }

    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    private static bool EnsureSingleInstance()
    {
        _mutex = new Mutex(true, "ChmlFrpAdvancedToolkit_GlobalMutex", out var createdNew);
        return createdNew;
    }

    public static void InitializeFirst()
    {
        Directory.CreateDirectory(Paths.DataPath);
        Directory.CreateDirectory(Paths.LogPath);
        Directory.CreateDirectory(Paths.PicturesPath);

        if (EnsureSingleInstance())
        {
            File.WriteAllText(Paths.LogfilePath, string.Empty);
            return;
        }

        var hWnd = FindWindow(null, "ChmlFrp Advanced Toolkit");
        if (hWnd != IntPtr.Zero) SetForegroundWindow(hWnd);
        Environment.Exit(0);
    }

    private static async void IsUpdate(bool isRemind = false)
    {
        if (isRemind) Reminders.Reminder_Box_Show("开始更新", "blue");
        Reminders.LogsOutputting("开始更新");

        var jObject = await Api.GetApi("http://cat.chmlfrp.com/update/update.json");

        if (jObject == null)
        {
            if (isRemind) Reminders.Reminder_Box_Show("更新失败", "red");
            Reminders.LogsOutputting("更新失败");
            return;
        }

        if (jObject!["version"]?.ToString() == Assembly.GetExecutingAssembly().GetName().Version.ToString())
        {
            if (isRemind) Reminders.Reminder_Box_Show("已是最新版本");
            Reminders.LogsOutputting("已是最新版本");
        }
        else
        {
            Reminders.Reminder_Box_Show("发现新版本", "blue");
            await Task.Delay(2000);
            Reminders.Reminder_Interface_Show(jObject["subject"]?.ToString(), jObject["text"]?.ToString(),
                true);
        }
    }

    private static void SetImage()
    {
        var imageFiles = Directory
            .GetFiles(Paths.PicturesPath, "*.*")
            .Where(file =>
                file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                || file.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
            )
            .ToArray();

        if (imageFiles.Length <= 0) return;

        Random random = new();
        var randomImage = imageFiles[random.Next(imageFiles.Length)];
        ViewsClass.MainWindowClass.Imagewallpaper.ImageSource = new BitmapImage(
            new Uri(randomImage, UriKind.RelativeOrAbsolute)
        );
        ViewsClass.MainWindowClass.Imagewallpaper.Stretch = Stretch.UniformToFill;
    }
}