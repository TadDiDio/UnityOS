using DeveloperConsole.Command;

namespace DeveloperConsole
{
    [ExcludeFromCmdRegistry(true)]
    [Command("fish", "Easter egg for Bucket of Fish team.")]
    public class FishCommand : SimpleCommand
    {
        protected override CommandOutput Execute(CommandContext context)
        {
            return new CommandOutput("You a good lookin' fish.");
        }
    }
}
