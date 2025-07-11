namespace DeveloperConsole.IO
{
    public interface IOutputChannel
    {
        public void Write(string token);
        public void WriteLine(string line);
    }
}
