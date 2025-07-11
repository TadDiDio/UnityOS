using DeveloperConsole.IO;

namespace DeveloperConsole.Core.Shell
{
    public class ShellRequest
    {
        public ICommandInput Input;
        public ShellSession Session;
        public bool Windowed;
    }
}
