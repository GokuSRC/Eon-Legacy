using Microsoft.UI.Xaml;

class GlobalSettings
{
    public static Window Windows;

    public static AppConfig Options;

    public static string Version = Definitions.CurrentVersion;

    public static bool SecretThemesUnlocked { get; set; } = false;
}