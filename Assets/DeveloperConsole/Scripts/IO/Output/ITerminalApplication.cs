namespace DeveloperConsole
{
    public interface ITerminalApplication : IOutputSink, IGraphical
    {
        public void OnInputRecieved(string input);
    }
}