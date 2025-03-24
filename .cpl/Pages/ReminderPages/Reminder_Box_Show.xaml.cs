using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ChmlFrp_Professional_Launcher.Pages
{
    /// <summary>
    /// Reminder_Box_Show.xaml 的交互逻辑
    /// </summary>
    public partial class Reminder_Box_Show : Page
    {
        public Reminder_Box_Show()
        {
            InitializeComponent();
            IsVisibleChanged += Reminder_Box_Show_IsVisibleChanged;
        }

        private void Reminder_Box_Show_IsVisibleChanged(
            object sender,
            DependencyPropertyChangedEventArgs e
        )
        {
            if (Visibility == Visibility.Visible)
            {
                StartTimer();
            }
        }

        private void StartTimer()
        {
            DispatcherTimer timer = new();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            Visibility = Visibility.Collapsed;
        }
    }
}
