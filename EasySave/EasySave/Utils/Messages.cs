using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Utils
{
    internal class Messages
    {
        private CultureInfo selectedCulture;
        private MessagesReader jsonMessagesReader;

        public static readonly CultureInfo FR = new("fr-FR");
        public static readonly CultureInfo EN = new("en-EN");

        public readonly List<CultureInfo> availableCultures =
        [
            EN,
            FR
        ];

        public string GetMessage(string messageKey)
        {
            return jsonMessagesReader.GetMessage(messageKey) ?? messageKey;
        }

        public Messages()
        {
            SetCulture(availableCultures[0]); // English by default
        }

        public Messages(CultureInfo culture)
        {
            SetCulture(culture);
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
}
