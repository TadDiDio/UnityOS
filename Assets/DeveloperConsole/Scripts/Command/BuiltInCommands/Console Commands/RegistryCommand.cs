using System.Collections.Generic;

namespace DeveloperConsole
{
    public class RegistryCommand : ConsoleCommand
    {
        protected override string Name() => "reg";
        protected override string Description() => "Shows a registry of all commands.";

        protected override CommandResult Execute(ConsoleCommandArgs args)
        {
            List<string> lines = new();

            foreach (var name in CommandRegistry.GetAllCommandNames())
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