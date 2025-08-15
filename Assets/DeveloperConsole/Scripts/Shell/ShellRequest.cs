using DeveloperConsole.Command;

namespace DeveloperConsole.Core.Shell
{
    public class ShellRequest
    {
        public ICommandResolver CommandResolver;
        public ShellSession Session;

        public bool Windowed;
        public bool ExpandAliases;
        public bool NoPrompt; // TODO: Change this for prompt filter to allow some but not all
    }
}
