namespace DeveloperConsole
{
    public interface IOutputSink
    {
        public void ReceiveOutput(string message);
    }
}