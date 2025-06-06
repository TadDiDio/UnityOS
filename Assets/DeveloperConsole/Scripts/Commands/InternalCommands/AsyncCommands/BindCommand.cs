using System;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeveloperConsole
{
    [Command("bind", "Manages system bindings.", true)]
    public class BindCommand : SimpleCommand
    {
        [Description("The type of the object you want to bind to.")]
        [PositionalArg(0)] 
        private Type type;

        [Description("The name of the object you want to bind to.")]
        [SwitchArg("name", 'n')] 
        private string name;
        
        [Description("The tag of the object you want to bind to.")]
        [SwitchArg("tag", 't')] 
        private string tag;
        
        [Subcommand] private ListBindingsCommand _listBindings;
        [Subcommand] private RemoveBindingCommand _removeBindings;
        
        protected override CommandResult Execute(CommandContext context)
        {
            var obj = ConsoleAPI.ResolveBinding(type, name, tag);
            return obj ? new CommandResult($"Binding set:  {type}  =>  {obj}") : new CommandResult("Binding could not be set.");
        }
    }
    
    [Command("list", "Lists all current bindings.", false)]
    public class ListBindingsCommand : SimpleCommand
    {
        [Description("The type of the object you want to show.")]
        [SwitchArg("type", 't')]
        private Type type = null;
        
        protected override CommandResult Execute(CommandContext context)
        {
            var bindings = ConsoleAPI.GetAllBindings();
            if (bindings.Count == 0) return new CommandResult("There are no bindings currently.");

            if (type == null)
            {
                List<string> lines = bindings.Select(kvp => $"{kvp.Key}  =>  {kvp.Value}").ToList();
                return new CommandResult(string.Join(Environment.NewLine, lines));
            }

            if (!bindings.TryGetValue(type, out Object obj))
            {
                return new CommandResult($"Binding for {type} not set.");
            }

            return new CommandResult($"{type}  =>  {obj}");
        }
    }
    
    [Command("click", "Binds to the object that is clicked.", false)]
    public class MouseSelectBindingCommand : AsyncCommand
    {
        [Description("The type of the object you want to bind to.")]
        [PositionalArg(0)]
        private Type type;
        
        public override async Task<CommandResult> ExecuteAsync(CommandContext context)
        {
            await Task.Delay(100);
            return new CommandResult("We would be clicking here.");
        }
    }
    
    [Command("remove", "Removes object bindings.", false)]
    public class RemoveBindingCommand : SimpleCommand
    {
        [Description("Which binding type to clear.")] 
        [PositionalArg(0)]
        private Type type;
        
        [Subcommand] private RemoveAllBindingsCommand _removeAllBindings;
        
        protected override CommandResult Execute(CommandContext context)
        {
            var bindings = ConsoleAPI.GetAllBindings();
            
            if (!bindings.Remove(type, out var obj))
            {
                return new CommandResult($"No binding of type '{type}'.");
            }

            return new CommandResult($"Binding '{type}  =>  {obj}' was removed.");
        }
    }

    [Command("all", "Clears all bindings.", false)]
    [ConfirmBeforeExecuting]
    public class RemoveAllBindingsCommand : SimpleCommand
    {
        protected override CommandResult Execute(CommandContext context)
        {
            var bindings = ConsoleAPI.GetAllBindings();
            if (bindings == null || bindings.Count == 0) return new CommandResult("There are no bindings currently.");
            
            bindings.Clear();
            return new CommandResult("All bindings cleared.");
        }
    }
}