using System.Linq;
using DeveloperConsole.Command;

namespace DeveloperConsole
{
    [Command("reg", "Shows a registry of all commands.")]
    public class RegistryCommand : SimpleCommand
    {
        [Optional(0, "Searches only the command tree for a specific command.")]
        private string command = null;
            
        [Switch('n', "Shows nested commands as well", "nested")]
        private bool _nested;
        
        protected override CommandOutput Execute(CommandContext context)
        {
            var names = ConsoleAPI.Commands.GetAllCommandNames();
            names.Sort();

            if (command is not null)
            {
                names = names.Where(n => n.StartsWith(command)).ToList();
                _nested = true;
            }
            
            if (!_nested)
            {
                names = names.Where(n => !n.Contains(".")).ToList();
            }
            
            var lines = names.Select(name => $"{name}: {ConsoleAPI.Commands.GetCommandDescription(name)}").ToList();
            var padded = MessageFormatter.PadFirstWordRight(lines);

            return new CommandOutput(padded);
        }
    }
}