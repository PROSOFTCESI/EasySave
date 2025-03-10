﻿namespace EasySave.Graphic3._0.ViewModel;

public class UserResponse(bool success, string messageId, params string[] args)
{
    public bool Success { get; set; } = success;
    public string MessageId { get; set; } = messageId;
    public string[]? Params { get; set; } = args;
    public bool Display { get; set; } = true;

    public static UserResponse GetEmptyUserResponse()
    {
        return new UserResponse(true, "", "")
        {
            Display = false,
        };
    }
}
