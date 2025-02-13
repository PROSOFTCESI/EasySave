using EasySave.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Graphic;

internal static class MessageBoxDisplayer
{
    public static void DisplayConfirmation(string messageId, params string[] args)
    {
        System.Windows.MessageBox.Show(
            string.Format(Messages.GetInstance().GetMessage(messageId), args),
            Messages.GetInstance().GetMessage("INFORMATION_MESSAGE"),
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Information);
    }
    public static void DisplayError(string messageId)
    {
        System.Windows.MessageBox.Show(
            Messages.GetInstance().GetMessage(messageId),
            Messages.GetInstance().GetMessage("ERROR_MESSAGE"),
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Error);
    }
}
