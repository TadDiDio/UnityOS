using System.Collections.Generic;
using DeveloperConsole.Command;

namespace DeveloperConsole
{
    [Command("alias", "Manages aliases for the session.")]
    public class AliasCommand : SimpleCommand
    {
        [Positional(0, "The alias key.")] private string key;
        [Positional(1, "The alias value.")] private string value;
        protected override CommandOutput Execute(SimpleContext context)
        {
            context.Session.AliasManager.AddAlias(key, value);
            return new CommandOutput($"Added alias {key} = {value}");
        }


        [Command("remove", "Removes an alias.")]
        public class RemoveCommand : SimpleCommand
        {
            [Positional(0, "The alias key")] private string key;
            protected override CommandOutput Execute(SimpleContext context)
            {
                context.Session.AliasManager.RemoveAlias(key);
                return new CommandOutput($"Removed alias {key}");
            }
        }

        [Command("list", "Lists all current aliases.")]
        public class ListCommand : SimpleCommand
        {
            protected override CommandOutput Execute(SimpleContext context)
            {
                List<(string, string)> lines = new();

                if (context.Session.AliasManager.GetAliases().Count == 0)
                {
                    return new CommandOutput("No aliases created.");
                }

                foreach (var kvp in context.Session.AliasManager.GetAliases())
                {
                    lines.Add(($"{kvp.Key}", $" = {kvp.Value}"));
                }

                return new CommandOutput(Formatter.PadLeft(lines));
            }
        }
    }
}
