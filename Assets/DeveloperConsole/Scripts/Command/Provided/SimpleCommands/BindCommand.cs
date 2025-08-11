using DeveloperConsole.Command;

namespace DeveloperConsole
{
    [Command("bind", "Tests the bind attribute")]
    public class BindCommand : SimpleCommand
    {
        [Bind]
        private TestClass test;
        protected override CommandOutput Execute(CommandContext context)
        {
            return new();
        }
    }
}
