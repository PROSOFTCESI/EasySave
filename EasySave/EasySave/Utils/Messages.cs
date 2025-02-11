using System.Globalization;

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
        EN,
        FR
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
        SetCulture(availableCultures[0]); // English by default
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
