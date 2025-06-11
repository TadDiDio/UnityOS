using System.Linq;
using System.Collections.Generic;

namespace DeveloperConsole
{
    // TODO: make a store option to add this to a file automatically.
    [Command("alias", "Makes aliases.", true)]
    public class AliasCommand : SimpleCommand
    {
        [Description("Alias name.")] [PositionalArg(0)]
        private string aliasName;

        [Description("The replacement.")] [VariadicArgs]
        private List<string> replacement; 
        
        [Subcommand]
        private AliasRemoveCommand removeSubcommand;
        
        [Subcommand]
        private AliasListCommand listSubcommand;
        
        protected override CommandResult Execute(CommandContext context)
        {
            context.Shell.AddAlias(aliasName, replacement);
            return new CommandResult($"Alias added: {aliasName}  =>  {string.Join(" ", replacement)}");
        }
    }
    
    [Command("remove", "Removes aliases.", false)]
    public class AliasRemoveCommand : SimpleCommand
    {
        [Description("The alias to remove")] [PositionalArg(0)]
        private string aliasName;
        
        protected override CommandResult Execute(CommandContext context)
        {
            if (!context.Shell.GetAliases().TryGetValue(aliasName, out _))
            {
                return new CommandResult($"There is no alias named '{aliasName}'");
            }
            
            context.Shell.RemoveAlias(aliasName);
            return new CommandResult($"Alias removed: {aliasName}");
        }
    }
    
    [Command("list", "Lists aliases.", false)]
    public class AliasListCommand : SimpleCommand
    {
        protected override CommandResult Execute(CommandContext context)
        {
            var aliases = context.Shell.GetAliases();

            if (aliases == null || aliases.Count == 0)
            {
                return new CommandResult("No aliases currently assigned.");
            }
            
            List<string> lines = new();

            var sorted = aliases.Keys.ToList();
            sorted.Sort();
            
            foreach (var name in sorted)
            {
                lines.Add($"{MessageFormatter.AddColor(name, MessageFormatter.Blue)}:  " +
                          $"{string.Join(" ", aliases[name])}");
            }
            
            string title = MessageFormatter.Title("Aliases", MessageFormatter.Green);
            string padded = MessageFormatter.PadFirstWordRight(lines);
            
            return new CommandResult($"{title}{padded}");
        }
    }
}