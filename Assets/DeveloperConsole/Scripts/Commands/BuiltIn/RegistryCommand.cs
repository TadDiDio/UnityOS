using System.Collections.Generic;

namespace DeveloperConsole.BuiltIn
{
    public class RegistryCommand : BuiltInCommand
    {
        protected override string Name() => "reg";
        protected override string Description() => "Shows a registry of all commands.";

        protected override CommandResult Execute(BuiltInCommandArgs args)
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