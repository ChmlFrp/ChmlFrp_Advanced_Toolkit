namespace CAT.Pages.ReminderPages;

public partial class ReminderBoxShow
{
    public ReminderBoxShow()
    {
        InitializeComponent();
        IsVisibleChanged += Reminder_Box_Show_IsVisibleChanged;
    }

    private void Reminder_Box_Show_IsVisibleChanged(
        object sender,
        DependencyPropertyChangedEventArgs e
    )
    {
        if (Visibility == Visibility.Visible) StartTimer();
    }

    private void StartTimer()
    {
        DispatcherTimer timer = new()
        {
            Interval = TimeSpan.FromSeconds(3)
        };
        timer.Tick += Timer_Tick;
        timer.Start();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        var timer = (DispatcherTimer)sender;
        timer.Stop();
        Visibility = Visibility.Collapsed;
    }
}