using IniParser;
using IniParser.Model;

namespace ChmlFrp_Advanced_Toolkit.Pages.SettingPages;

public partial class PersonalizationPage
{
    private readonly FileIniDataParser _parser = new();
    private readonly IniData _data;
    private readonly string _theme;
    
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
        _data = _parser.ReadFile(Paths.InifilePath);
        _theme = _data["CAT_Config"]["Theme"];
        switch (_theme)
        {
            case "ChmlFrp":
                ChmlFrpButton.IsEnabled = true;
                ChmlFrp_Click(null, null);
                break;
            case "Chaoji":
                ChaojiButton.IsEnabled = true;
                Chaoji_Click(null, null);
                break;
        }
    }

    private void ChmlFrp_Click(object sender, RoutedEventArgs e)
    {
        var resourceDictionary = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/ChmlFrp Advanced Toolkit;component/Themes/Theme.xaml")
        };
        if (resourceDictionary["ThemeColor"] is SolidColorBrush themeColorBrush)
            themeColorBrush.Color = (Color)ColorConverter.ConvertFromString("#116fce")!;
        Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);

        if (_theme == "ChmlFrp") return;

        _data["CAT_Config"]["Theme"] = "ChmlFrp";
        _parser.WriteFile(Paths.InifilePath, _data);
    }


    private void Chaoji_Click(object sender, RoutedEventArgs e)
    {
        var resourceDictionary = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/ChmlFrp Advanced Toolkit;component/Themes/Theme.xaml")
        };
        if (resourceDictionary["ThemeColor"] is SolidColorBrush themeColorBrush)
            themeColorBrush.Color = (Color)ColorConverter.ConvertFromString("#F9A640")!;
        Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
        
        if (_theme == "Chaoji") return;

        _data["CAT_Config"]["Theme"] = "Chaoji";
        _parser.WriteFile(Paths.InifilePath, _data);
    }
}