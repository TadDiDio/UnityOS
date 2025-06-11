using System.Collections.Generic;

namespace DeveloperConsole
{
    [Command("reg", "Shows a registry of all commands.", true)]
    public class RegistryCommand : SimpleCommand
    {
        protected override CommandResult Execute(CommandContext args)
        {
            List<string> lines = new();

            var sorted = ConsoleAPI.GetBaseCommandNames();
            sorted.Sort();
            
            foreach (var name in sorted)
            {
                lines.Add($"{MessageFormatter.AddColor(name, MessageFormatter.Blue)}: " +
                          $"{ConsoleAPI.GetCommandDescription(name)}");
            }
            
            string title = MessageFormatter.Title("Commands", MessageFormatter.Green);
            string padded = MessageFormatter.PadFirstWordRight(lines);
            
            return new CommandResult($"{title}{padded}");
        }
    }
}