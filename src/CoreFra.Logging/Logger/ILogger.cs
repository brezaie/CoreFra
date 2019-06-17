namespace CoreFra.Logging
{
    public interface ILogger
    {
        void ErrorException(string message, System.Exception exception);
        void Error(string message);
    }
}