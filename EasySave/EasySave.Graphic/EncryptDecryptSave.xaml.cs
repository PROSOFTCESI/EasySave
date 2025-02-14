using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EasySave.Utils;
using CryptoSoftLib;

namespace EasySave.Graphic
{
    /// <summary>
    /// Logique d'interaction pour EncryptDecryptSave.xaml
    /// </summary>
    public partial class EncryptDecryptSave : Page
    {
        private readonly Messages message = Messages.GetInstance();
        private SettingsJsonDefinition settings;
        public EncryptDecryptSave()
        {
            InitializeComponent();
            settings = SettingsJson.GetInstance().GetContent();

            SaveRepLabel.Text = message.GetMessage("ASK_SAVE_JOB_SOURCE_MESSAGE");
            KeyLabel.Text = message.GetMessage("ENCRYPTION_KEY");
            EncryptButton.Content = message.GetMessage("ENCRYPTION");
            BackButton.Content = message.GetMessage("BACK");

            SaveRepTextBox.Text = "";
            KeyTextBox.Text = settings.EncryptionKey;
        }

        private void EncryptDecrypt_Click(object sender, RoutedEventArgs e)
        {
            CryptoSoft.EncryptDecryptFolder(SaveRepTextBox.Text, KeyTextBox.Text);

        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
