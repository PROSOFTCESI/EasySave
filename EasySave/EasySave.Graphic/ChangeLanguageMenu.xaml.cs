using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using EasySave.Utils;

namespace EasySave.Graphic
{
    public partial class ChangeLanguageMenu : Page
    {
        private Messages messages = Messages.GetInstance();

        private string parent;

        public ChangeLanguageMenu(string parent = "MAIN")
        {
            InitializeComponent();
            PopulateLanguageButtons();
            TitleLabel.Text = messages.GetMessage("CHANGE_LANGUAGE_MENU_LABEL");
            BackButton.Content = messages.GetMessage("BACK");

            this.parent = parent;            
        }


        private void PopulateLanguageButtons()
        {
            foreach (CultureInfo culture in Messages.availableCultures)
            {
                string displayName = Regex.Replace(culture.DisplayName, @"\s*\(.*?\)", "");
                Button languageButton = new Button
                {

                    Content = char.ToUpper(displayName[0]) + displayName.Substring(1),
                    Width = 200,
                    Margin = new Thickness(5)
                };
                languageButton.Click += (sender, e) => SetLanguage(culture);
                LanguageStackPanel.Children.Add(languageButton);
            }
        }

        private void SetLanguage(CultureInfo culture)
        {
            messages.SetCulture(culture);
            TitleLabel.Text = Messages.GetInstance().GetMessage("CHANGE_LANGUAGE_MENU_LABEL");
            BackButton.Content = messages.GetMessage("BACK");
            MessageBoxDisplayer.DisplayConfirmation("LANGUAGE_CHANGED_SUCCESSFULLY");
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            if(parent == "Settings")           
                NavigationService.Navigate(new SettingsMenu());
            
            if(parent == "MAIN")
                NavigationService.Navigate(new MainMenu());
        }
    }
}
