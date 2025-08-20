using DeveloperConsole.Command;
using DeveloperConsole.Windowing;

namespace DeveloperConsole.Core.Shell
{
    public class ShellRequest
    {
        public ICommand Command;
        public ShellSession Session;

        // TODO WINDOW
        // public CommandWindow Window = null;
        public bool NoPrompt; // TODO: Change this for prompt filter to allow some but not all
    }
}
