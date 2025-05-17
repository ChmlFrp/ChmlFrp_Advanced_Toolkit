using System.Windows.Media;
using CAT.Views.Pages.ReminderPages;

namespace CAT.Views.Windows.Classes;

public static class Reminders
{
    private static readonly ReminderBoxShow Rbsw = new();
    private static readonly ReminderInterfaceShow Risw = new();
    private static readonly ReminderDownloadShow Rdsw = new();

    public static void LogsOutputting(string logEntry)
    {
        logEntry = $"[{DateTime.Now}] " + logEntry;

        Console.WriteLine(logEntry);
        File.AppendAllText(Paths.LogfilePath, logEntry + Environment.NewLine);
    }

    public static void Reminder_Box_Show(string message, string color = "green")
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            LogsOutputting("提醒消息不能为空或全为空格");
            return;
        }

        if (Rbsw.Visibility == Visibility.Collapsed)
            Rbsw.Visibility = Visibility.Visible;

        switch (color)
        {
            case "green":
                Rbsw.RemindersBorder.Background = new SolidColorBrush(
                    Colors.LimeGreen
                );
                break;
            case "blue":
                Rbsw.RemindersBorder.Background = new SolidColorBrush(
                    Colors.DodgerBlue
                );
                break;
            case "red":
                Rbsw.RemindersBorder.Background = new SolidColorBrush(
                    Colors.Red
                );
                break;
            case "yellow":
                Rbsw.RemidingTextBlock.Foreground = new SolidColorBrush(
                    Colors.Green
                );
                Rbsw.RemindersBorder.Background = new SolidColorBrush(
                    Colors.Yellow
                );
                break;
        }

        Rbsw.RemidingTextBlock.Text = message;
        ViewsClass.MainWindowClass.RemindersNavigationTwo.Navigate(Rbsw);
    }

    public static void Reminder_Interface_Show(
        string subject,
        string message,
        bool isUpdate = false
    )
    {
        if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(message))
        {
            LogsOutputting("提醒消息不能为空或全为空格");
            return;
        }

        if (Risw.Visibility == Visibility.Collapsed)
            Risw.Visibility = Visibility.Visible;

        Risw.YesCornerButten.IsEnabled = true;

        if (isUpdate)
        {
            Risw.YesCornerButten.Content = "更新";
            Risw.YesCornerButten.Click += Risw.Update;
        }
        else
        {
            Risw.YesCornerButten.Click += Risw.Close;
        }

        Risw.SubjectTextBlock.Text = subject;
        Risw.TextTextBlock.Text = message;
        ViewsClass.MainWindowClass.RemindersNavigation.Navigate(Risw);
    }

    public static void Reminder_Download_Show()
    {
        if (Rdsw.Visibility == Visibility.Collapsed)
            Rdsw.Visibility = Visibility.Visible;

        ViewsClass.MainWindowClass.RemindersNavigation.Navigate(Rdsw);
    }
}