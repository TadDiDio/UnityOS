using System.Collections.Generic;

namespace DeveloperConsole
{
    [Command("reg", "Shows a registry of all commands.")]
    public class RegistryCommand : ConsoleCommand
    {
        protected override CommandResult Execute(ConsoleCommandArgs args)
        {
            List<string> lines = new();

            foreach (var name in CommandRegistry.GetBaseCommandNames())
            {
                lines.Add($"{MessageFormatter.AddColor(name, MessageFormatter.Blue)}: " +
                          $"{CommandRegistry.GetDescription(name)}");
            }
            
            string title = MessageFormatter.Title("Commands", MessageFormatter.Green);
            string padded = MessageFormatter.PadFirstWordRight(lines);
            
            return new CommandResult($"{title}{padded}");
        }
    }
}