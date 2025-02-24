namespace EasySave.CustomExceptions;

public class PausedSaveException : Exception
{
    public PausedSaveException()
    {
    }

    public PausedSaveException(string message)
        : base(message)
    {
    }

    public PausedSaveException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
