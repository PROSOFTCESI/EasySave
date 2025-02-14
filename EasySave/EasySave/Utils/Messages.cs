using System.Globalization;
using EasySave.Utils;

namespace EasySave.Utils;

public class Messages
{
    private static Messages? Instance = null;

    private CultureInfo selectedCulture;
    private MessagesReader jsonMessagesReader;

    public static readonly CultureInfo FR = new("fr-FR");
    public static readonly CultureInfo EN = new("en-EN");

    public static readonly List<CultureInfo> availableCultures =
    [
        FR,
        EN
    ];

    public string GetMessage(string messageKey)
    {
        return jsonMessagesReader.GetMessage(messageKey) ?? messageKey;
    }

    public static Messages GetInstance()
    {
        Instance ??= new Messages();
        return Instance;
    }
    private Messages()
    {
        SetCulture(new CultureInfo(SettingsJson.GetInstance().GetContent().selectedCulture));
    }

    public void SetCulture(CultureInfo culture)
    {
        if (!IsCultureSupported(culture))
        {
            throw new CultureNotFoundException("Culture not supported");
        }
        selectedCulture = culture;
        jsonMessagesReader = new MessagesReader(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Utils", "Localization", "Messages"),
            GetJsonName(selectedCulture)
        );

        SettingsJsonDefinition settings = SettingsJson.GetInstance().GetContent();
        settings.selectedCulture = culture.Name;
        SettingsJson.GetInstance().Update(settings);
    }
    private bool IsCultureSupported(CultureInfo culture)
    {
        return availableCultures.Contains(culture);
    }
    private string GetJsonName(CultureInfo culture)
    {
        string jsonPath = "messages.{0}.json";
        jsonPath = selectedCulture.Name switch
        {
            "en-EN" => string.Format(jsonPath, "en"),
            "fr-FR" => string.Format(jsonPath, "fr"),
            _ => throw new CultureNotFoundException("Culture not supported")
        };
        return jsonPath;
    }
}
