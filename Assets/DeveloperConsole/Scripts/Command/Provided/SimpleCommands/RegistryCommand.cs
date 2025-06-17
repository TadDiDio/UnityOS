using System.Linq;
using DeveloperConsole.Command;

namespace DeveloperConsole
{
    [Command("reg", "Shows a registry of all commands.")]
    public class RegistryCommand : SimpleCommand
    {
        // TODO:
        //[OptionalPositional()]
        
        [Switch('n', "Shows nested commands as well")]
        private bool _nested;
        
        protected override CommandOutput Execute(CommandContext context)
        {
            var names = ConsoleAPI.Commands.GetAllCommandNames();
            names.Sort();

            if (!_nested)
            {
                names = names.Where(n => !n.Contains(".")).ToList();
            }
            
            var lines = names.Select(name => $"{name}: {ConsoleAPI.Commands.GetCommandDescription(name)}").ToList();
            var result = MessageFormatter.PadFirstWordRight(lines);

            return new CommandOutput(result);
        }
    }
}