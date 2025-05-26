namespace DeveloperConsole
{
    public interface IConsoleOutputSink
    {
        public void ReceiveOutput(string message);
    }
}