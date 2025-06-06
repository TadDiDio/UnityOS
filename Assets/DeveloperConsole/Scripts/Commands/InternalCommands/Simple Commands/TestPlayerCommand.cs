namespace DeveloperConsole
{
    [Command("player", "Tests the bindings system.", true)]
    public class TestPlayerCommand : SimpleCommand
    {
        [Binding("Player (3)", "Player")] 
        private Player player;
        protected override CommandResult Execute(CommandContext context)
        {
            return new CommandResult($"The player's name is {player.Name}!");
        }
    }
}