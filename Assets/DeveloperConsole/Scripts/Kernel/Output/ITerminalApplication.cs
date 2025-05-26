namespace DeveloperConsole
{
    public interface ITerminalApplication : IConsoleOutputSink, IGraphical
    {
        public void OnInput(string input);
    }
}