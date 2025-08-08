using System.Collections.Generic;
using DeveloperConsole.Command;

namespace DeveloperConsole
{
    [Command("alias", "Manages aliases for the session.")]
    public class AliasCommand : SimpleCommand
    {
        [Positional(0, "The alias key.")] private string key;
        [Positional(1, "The alias value.")] private string value;
        protected override CommandOutput Execute(CommandContext context)
        {
            context.Session.AddAlias(key, value);
            return new CommandOutput($"Added alias {key} = {value}");
        }


        [Subcommand("remove", "Removes an alias.", typeof(AliasCommand))]
        public class RemoveCommand : SimpleCommand
        {
            [Positional(0, "The alias key")] private string key;
            protected override CommandOutput Execute(CommandContext context)
            {
                context.Session.RemoveAlias(key);
                return new CommandOutput($"Removed alias {key}");
            }
        }

        [Subcommand("list", "Lists all current aliases.", typeof(AliasCommand))]
        public class ListCommand : SimpleCommand
        {
            protected override CommandOutput Execute(CommandContext context)
            {
                List<(string, string)> lines = new();

                if (context.Session.GetAliases().Count == 0)
                {
                    return new CommandOutput("No aliases created.");
                }

                foreach (var kvp in context.Session.GetAliases())
                {
                    lines.Add(($"{kvp.Key}", $" = {kvp.Value}"));
                }

                return new CommandOutput(MessageFormatter.PadLeft(lines));
            }
        }
    }
}
