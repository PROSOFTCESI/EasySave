using EasySave.Graphic3._0.ViewModel;
using EasySave.Utils;
using LoggerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Graphic;

internal static class MessageBoxDisplayer
{
    public static void Display(UserResponse response)
    {
        if (!response.Success)
        {
            DisplayError(response.MessageId, response.Params);
            return;
        }
        DisplayConfirmation(response.MessageId, response.Params);
    }

    public static void DisplayConfirmation(string messageId, params string[]? args)
    {
        System.Windows.MessageBox.Show(
            string.Format(Messages.GetInstance().GetMessage(messageId), args),
            Messages.GetInstance().GetMessage("INFORMATION_MESSAGE"),
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Information);
    }
    public static void DisplayError(string messageId, params string[]? args)
    {
        Logger.GetInstance().Log(
        new
        {
           Type = "Message",
           Time = DateTime.Now,
           Statut = "Error",
           Message = Messages.GetInstance().GetMessage(messageId),
        });

        System.Windows.MessageBox.Show(
            string.Format(Messages.GetInstance().GetMessage(messageId), args),
            Messages.GetInstance().GetMessage("ERROR_MESSAGE"),
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Error);
    }
}
