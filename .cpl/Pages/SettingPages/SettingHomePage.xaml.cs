using System.Windows.Controls;

namespace ChmlFrp_Professional_Launcher.Pages
{
    /// <summary>
    /// SettingHomePage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingHomePage : Page
    {
        public SettingHomePage()
        {
            InitializeComponent();
            MainClass.Reminders.LogsOutputting("进入SettingHomePage");
        }
    }
}
