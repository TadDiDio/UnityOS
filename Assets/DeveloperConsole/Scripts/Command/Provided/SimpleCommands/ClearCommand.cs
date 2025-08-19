using DeveloperConsole.Command;
using DeveloperConsole.Core.Shell;

namespace DeveloperConsole
{
    [Command("Clear", "Clears the screen.")]
    public class ClearCommand : SimpleCommand
    {
        protected override CommandOutput Execute(SimpleContext context)
        {
            Signal(new ClearSignal());
            return new CommandOutput();
        }
    }
}
