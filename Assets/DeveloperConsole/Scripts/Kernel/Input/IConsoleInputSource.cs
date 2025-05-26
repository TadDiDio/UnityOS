namespace DeveloperConsole
{
    public interface IConsoleInputSource
    {
        public bool InputAvailable();
        public string GetInput();
    }
}