using Microsoft.Win32;

namespace CAT.Views.Windows.Classes;

public static class User
{
    private static readonly RegistryKey Key =
        Registry.CurrentUser.CreateSubKey(@"SOFTWARE\\ChmlFrp", true);

    public static string Username;
    public static string Password;
    public static string Usertoken;
    public static volatile bool SignInBool;

    static User()
    {
        Load();
    }

    private static void Load()
    {
        Username = Key.GetValue("username")?.ToString();
        Password = Key.GetValue("password")?.ToString();
        Usertoken = Key.GetValue("usertoken")?.ToString();
    }

    public static void Save(string username, string password, string usertoken)
    {
        Key.SetValue("username", username);
        Key.SetValue("password", password);
        Key.SetValue("usertoken", usertoken);
        Load();
    }
}