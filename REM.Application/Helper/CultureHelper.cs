using System.Text.Json;

namespace REM.Application.Helper;

public static class CultureHelper
{
    private static readonly string _resourcesPath = Path.Combine(AppContext.BaseDirectory, "Resources", "Localization");
    private static Dictionary<string, Dictionary<string, string>> _localizationData = [];

    public static string CurrentLanguage { get; set; } = "en";
    public static bool IsArabic => CurrentLanguage == "ar";

    static CultureHelper() => LoadMessages();

    public static void SetLanguage(string language)
    {
        CurrentLanguage = language;
        LoadMessages();
    }

    public static string GetResource(string category, string key)
    {
        return
            _localizationData.ContainsKey(category) && _localizationData[category].ContainsKey(key)
            ? _localizationData[category][key]
            : $"[{key}]"; // Default to showing the key if not found
    }

    private static void LoadMessages()
    {
        var filePath = Path.Combine(_resourcesPath, $"{CurrentLanguage}.json");

        if (!File.Exists(filePath))
        {
            _localizationData = [];
            return;
        }

        var json = File.ReadAllText(filePath);
        _localizationData =
            JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json) ?? [];
    }

    // this method should be removed and use the json file instead
    // public static string GetMessage(string key) =>
    //     CommonResource.ResourceManager.GetString(key, Thread.CurrentThread.CurrentCulture)!;
}
