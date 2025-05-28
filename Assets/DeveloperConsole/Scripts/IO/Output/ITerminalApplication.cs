namespace DeveloperConsole
{
    public interface ITerminalApplication : IConsoleOutputSink, IGraphical
    {
        public void OnInputRecieved(string input);
    }
}