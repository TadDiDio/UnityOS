using System.Linq;
using DeveloperConsole.Command;

namespace DeveloperConsole
{
    [Command("reg", "Shows a registry of all commands.")]
    public class RegistryCommand : SimpleCommand
    {
        [Optional(0, "Displays all commands and their subcommands that start with the this string.")]
        private string filter = null;

        [Switch('n', "Shows nested commands as well", "nested")]
        private bool nested;

        protected override CommandOutput Execute(SimpleCommandContext context)
        {
            var names = ConsoleAPI.Commands.GetAllCommandNames();
            names.Sort();

            if (filter is not null)
            {
                names = names.Where(n => n.StartsWith(filter)).ToList();
                nested = true;
            }

            if (!nested)
            {
                names = names.Where(n => !n.Contains(".")).ToList();
            }

            var lines = names.Select(name => ($"{name}:", $"{ConsoleAPI.Commands.GetCommandDescription(name)}")).ToList();
            var padded = Formatter.PadLeft(lines);

            return new CommandOutput(padded);
        }
    }
}
