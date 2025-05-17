using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace CAT.Views.Windows.Classes;

public static class Api
{
    public static async Task<bool> Downloadasync(string url, string path)
    {
        if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(path))
        {
            Reminders.LogsOutputting("下载失败：NullOrWhiteSpace");
            return false;
        }

        try
        {
            using WebClient client = new();
            client.Encoding = Encoding.UTF8;
            await client.DownloadFileTaskAsync(new Uri(url), path);
        }
        catch
        {
            Reminders.LogsOutputting($"下载失败：文件占用或网络错误?&url={url}");
            return false;
        }

        return true;
    }

    public static async Task<JObject> GetApi(string url, Dictionary<string, string> parameters = null)
    {
        if (parameters != null)
            url = $"{url}?{string.Join("&", parameters.Select(pair => $"{pair.Key}={pair.Value}"))}";

        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var jObject = JObject.Parse(await response.Content.ReadAsStringAsync());
            return jObject;
        }
        catch (Exception ex)
        {
            Reminders.LogsOutputting($"请求错误: {ex.Message}");
            return null;
        }
    }

    public static async Task<bool> GetApItoLogin(bool isRemind = true, string name = "", string password = "")
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
        {
            password = User.Password;
            name = User.Username;
        }

        var parameters = new Dictionary<string, string>
        {
            { "username", $"{name}" },
            { "password", $"{password}" }
        };

        var jObject = await GetApi("https://cf-v2.uapis.cn/login", parameters);

        if (jObject == null)
        {
            if (isRemind) Reminders.Reminder_Box_Show("网络错误", "red");
            return false;
        }

        var msg = jObject["msg"]?.ToString();
        Reminders.LogsOutputting("API提醒：" + msg);
        if (msg != "登录成功")
        {
            if (isRemind) Reminders.Reminder_Box_Show(msg, "red");
            return false;
        }

        if (isRemind) Reminders.Reminder_Box_Show(msg);

        User.Save(name, password, jObject["data"]?["usertoken"]?.ToString());
        User.SignInBool = true;
        return true;
    }
}