using System.Collections.Generic;

namespace DeveloperConsole
{
    [Command("reg", "Shows a registry of all commands.", false)]
    public class RegistryCommand : SimpleCommand
    {
        protected override CommandResult Execute(CommandContext args)
        {
            List<string> lines = new();

            foreach (var name in Kernel.Instance.Get<ICommandRegistryProvider>().GetBaseCommandNames())
            {
                lines.Add($"{MessageFormatter.AddColor(name, MessageFormatter.Blue)}: " +
                          $"{Kernel.Instance.Get<ICommandRegistryProvider>().GetDescription(name)}");
            }
            
            string title = MessageFormatter.Title("Commands", MessageFormatter.Green);
            string padded = MessageFormatter.PadFirstWordRight(lines);
            
            return new CommandResult($"{title}{padded}");
        }
    }
}