using System.Windows.Controls;

namespace CAT.Views.Pages.LauchingPages;

public partial class LaunchPage
{
    public LaunchPage()
    {
        InitializeComponent();
        OnSelectionChanged(null, null);
    }

    private void Launchingthis(object sender, RoutedEventArgs e)
    {
        LaunchingButton.Click -= Launchingthis;
        TunnelComboBox.IsEnabled = false;

        Frp.OnFrpStatusChanged += () => OnSelectionChanged(null, null);
        Frp.StartFrp(TunnelComboBox.Text, LaunchingButton);
    }

    private void Closingthis(object sender, RoutedEventArgs e)
    {
        LaunchingButton.Click -= Closingthis;
        TunnelComboBox.IsEnabled = false;

        Frp.OnFrpStatusChanged += () => OnSelectionChanged(null, null);
        Frp.CloseFrp(TunnelComboBox.Text);
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        LaunchingButton.Click -= Closingthis;
        LaunchingButton.Click -= Launchingthis;
        LaunchingButton.IsEnabled = true;
        TunnelComboBox.IsEnabled = true;
        var selectedItem = TunnelComboBox.SelectedItem as string;

        if (string.IsNullOrEmpty(selectedItem))
        {
            LaunchingButton.Content = "未选择隧道";
            LaunchingButton.IsEnabled = false;
            return;
        }

        if (!Frp.FrpExists(selectedItem))
        {
            LaunchingButton.Content = "启动FRPC";
            LaunchingButton.Click += Launchingthis;
            TunnelComboBox.IsEnabled = true;
            return;
        }

        TunnelComboBox.IsEnabled = true;
        LaunchingButton.Content = "关闭FRPC";
        LaunchingButton.Click += Closingthis;
    }
}