using EasySave.Graphic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EasySave.Graphic3._0.ViewModel;

internal class UserResponse(bool success, string messageId, params string[] args)
{
    public bool Success { get; set; } = success;
    public string MessageId { get; set; } = messageId;
    public string[]? Params { get; set; } = args;
}
