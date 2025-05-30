namespace DeveloperConsole
{
    public interface IInputSource
    {
        public bool InputAvailable();
        public string GetInput();
    }
}