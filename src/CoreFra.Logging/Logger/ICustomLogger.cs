namespace CoreFra.Logging
{
    public interface ICustomLogger
    {
        void ErrorException(string message, System.Exception exception);
        void Error(string message);
    }
}