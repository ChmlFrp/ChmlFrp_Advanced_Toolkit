namespace CAT.Views.Windows.Classes;

public abstract class Paths
{
    public static readonly string DataPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ChmlFrp");

    public static readonly string LogPath = Path.Combine(DataPath, "Logs");
    public static readonly string FrpExePath = Path.Combine(DataPath, "frpc.exe");
    public static readonly string PicturesPath = Path.Combine(DataPath, "Pictures");
    public static readonly string InifilePath = Path.Combine(DataPath, "CAT_Config.ini");
    public static readonly string LogfilePath = Path.Combine(DataPath, "Debug-CAT.logs");
}