using System.IO;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;

namespace ChmlFrp_Professional_Launcher.Pages.ChmlFrpLoginPages
{
    /// <summary>
    /// TMAPage.xaml 的交互逻辑
    /// </summary>
    public partial class TMAPage : Page
    {
        public TMAPage()
        {
            InitializeComponent();
            new MainClass.User();
            InitializeApps();
        }

        public async void InitializeApps()
        {
            if (
                await MainClass.Downloadfiles.Downloadasync(
                    "http://cf-v2.uapis.cn/tunnel?token=" + MainClass.User.usertoken,
                    MainClass.Paths.Temp.temp_api_tunnel
                )
            )
            {
                var jsonObject = JObject.Parse(
                    File.ReadAllText(MainClass.Paths.Temp.temp_api_tunnel)
                );
                if (jsonObject["msg"]?.ToString() == "获取隧道数据成功")
                {
                    int index = 0;
                    foreach (var tunnel in jsonObject["data"])
                    {
                        string tunnelname = tunnel["name"].ToString();
                        string tunnelid = tunnel["id"].ToString();
                        string tunneltype = tunnel["type"].ToString();
                        string tunnelstate = tunnel["state"].ToString();
                        string tunnelip = tunnel["ip"].ToString();
                        string tunnelnport = tunnel["nport"].ToString();
                        string tunneldorp = tunnel["dorp"].ToString();

                        switch (index)
                        {
                            case 1:
                            {
                                break;
                            }
                            case 2:
                            {
                                break;
                            }
                            case 3:
                            {
                                break;
                            }
                            case 4:
                            {
                                break;
                            }
                        }

                        index++;
                    }
                }
            }
        }

        private void btnRefresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            InitializeApps();
        }
    }
}
