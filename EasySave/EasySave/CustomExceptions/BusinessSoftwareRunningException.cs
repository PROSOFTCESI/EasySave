namespace EasySave.CustomExceptions;

public class BusinessSoftwareRunningException : Exception
{
    public BusinessSoftwareRunningException()
    {
    }

    public BusinessSoftwareRunningException(string message)
        : base(message)
    {
    }

    public BusinessSoftwareRunningException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
