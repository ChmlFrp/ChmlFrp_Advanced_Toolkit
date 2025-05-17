using System.Windows.Media;
using IniParser.Model;

namespace CAT.Views.Pages.SettingPages;

public partial class PersonalizationPage
{
    private readonly IniData _data;
    private readonly FileIniDataParser _parser = new();
    private string _theme;

    public PersonalizationPage()
    {
        InitializeComponent();
        if (!File.Exists(Paths.InifilePath))
        {
            _data = new IniData
            {
                ["CAT_Config"] =
                {
                    ["Theme"] = "ChmlFrp"
                }
            };
            _parser.WriteFile(Paths.InifilePath, _data);
        }

        if (DateTime.Today is { Month: 4, Day: 1 })
        {
            ChmlFrpButton.IsEnabled = false;
            ChaojiButton.IsEnabled = false;

            ChengeTheme(Colors.LightGreen);
            return;
        }

        _data = _parser.ReadFile(Paths.InifilePath);
        _theme = _data["CAT_Config"]["Theme"];
        switch (_theme)
        {
            case "ChmlFrp":
                ChmlFrpButton.IsChecked = true;
                ChmlFrp_Click(null, null);
                break;
            case "Chaoji":
                ChaojiButton.IsChecked = true;
                Chaoji_Click(null, null);
                break;
            case "Official":
                OfficialButton.IsChecked = true;
                Official_Click(null, null);
                break;
        }
    }

    private void ChengeTheme(Color color, string theme = "")
    {
        var resourceDictionary = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/ChmlFrp Advanced Toolkit;component/Theme.xaml")
        };
        if (resourceDictionary["ThemeColor"] is SolidColorBrush themeColorBrush)
            themeColorBrush.Color = color;
        Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);

        if (_theme == theme || theme == "") return;
        _theme = theme;
        _data["CAT_Config"]["Theme"] = theme;
        _parser.WriteFile(Paths.InifilePath, _data);
    }

    private void ChmlFrp_Click(object sender, RoutedEventArgs e)
    {
        ChengeTheme((Color)ColorConverter.ConvertFromString("#116fce")!, "ChmlFrp");
    }

    private void Chaoji_Click(object sender, RoutedEventArgs e)
    {
        ChengeTheme((Color)ColorConverter.ConvertFromString("#ff7f00")!, "Chaoji");
    }


    private void Official_Click(object sender, RoutedEventArgs e)
    {
        ChengeTheme((Color)ColorConverter.ConvertFromString("#722ED1")!, "Official");
    }
}