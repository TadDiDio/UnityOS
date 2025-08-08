using DeveloperConsole.Command;

namespace DeveloperConsole.Core.Shell
{
    public class ShellRequest
    {
        public bool Windowed;
        public bool ExpandAliases;
        public ShellSession Session;
        public ICommandResolver CommandResolver;
    }
}
