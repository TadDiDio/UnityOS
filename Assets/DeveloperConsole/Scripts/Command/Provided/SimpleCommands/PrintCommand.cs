using System.Collections.Generic;
using DeveloperConsole.Command;

namespace DeveloperConsole
{
    [Command("print", "Prints a message to the console.")]
    public class PrintCommand : SimpleCommand
    {
        [Variadic("The message to display")]
        public List<string> message;

        protected override CommandOutput Execute(SimpleContext context)
        {
            string m = string.Join(" ", message);
            return new CommandOutput(m);
        }
    }
}
