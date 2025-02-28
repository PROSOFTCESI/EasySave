namespace EasySave.CustomExceptions;

public class PlayPauseStopException : Exception
{
    public PlayPauseStopException()
    {
    }

    public PlayPauseStopException(string message)
        : base(message)
    {
    }

    public PlayPauseStopException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
