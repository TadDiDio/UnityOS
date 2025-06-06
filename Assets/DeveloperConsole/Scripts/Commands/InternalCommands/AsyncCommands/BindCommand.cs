using System;
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
        
        [Subcommand] private ShowBindingsCommand showBindings;
        [Subcommand] private ClearBindingCommand clearBindings;
        
        protected override CommandResult Execute(CommandContext context)
        {
            var obj = ConsoleAPI.ResolveBinding(type, name, tag);

            if (obj)
            {
                return new CommandResult($"Binding set:  {type}  =>  {obj}");
            }
            
            return new CommandResult("Binding could not be set.");
        }
    }
    
    [Command("show", "Shows all current bindings.", false)]
    public class ShowBindingsCommand : SimpleCommand
    {
        protected override CommandResult Execute(CommandContext context)
        {
            var bindings = ConsoleAPI.GetAllBindings();
            if (bindings.Count == 0) return new CommandResult("There are no bindings currently.");
            
            List<string> lines = bindings.Select(kvp => $"{kvp.Key}  =>  {kvp.Value}").ToList();

            return new CommandResult(string.Join(Environment.NewLine, lines));
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
    
    [Command("clear", "Clears object bindings.", false)]
    public class ClearBindingCommand : AsyncCommand
    {
        [Description("Clears all bindings.")]
        [SwitchArg("all", 'a')]
        private bool all;

        [Description("Which binding type to clear.")] 
        [SwitchArg("type", 't')]
        private Type type;
        
        public override async Task<CommandResult> ExecuteAsync(CommandContext context)
        {
            if (context.Tokens.Count == 2)
            {
                // TODO: Replace with choice functionality when added.
                context.Shell.SendOutput("Do you want to clear all bindings? (y/n)", false);
                var choice = await context.Shell.WaitForInput();
                context.Shell.SendOutput(choice, true);
                
                if (choice.Trim().ToLower() != "y")
                {
                    return new CommandResult("Operation cancelled.");
                }

                all = true;
            }
            
            var bindings = ConsoleAPI.GetAllBindings();
            if (all)
            {
                bindings.Clear();
                return new CommandResult("All bindings cleared.");
            }
            
            if (!bindings.TryGetValue(type, out var obj))
            {
                return new CommandResult($"No binding of type '{type}'.");
            }

            bindings.Remove(type);
            return new CommandResult($"Binding '{type}  =>  {obj}' was removed.");
        }
    }
}