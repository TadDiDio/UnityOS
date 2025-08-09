using System;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Linq;
using DeveloperConsole.Command;

namespace DeveloperConsole
{
    [Command("bind", "Manages system bindings.")]
    public class BindCommand : SimpleCommand
    {
        [Positional(0, "The type of the object you want to bind to.")]
        private Type type;

        [Switch('n', "The name of the object you want to bind to.")]
        private string name;

        [Switch('t', "The tag of the object you want to bind to.")]
        private string tag;

        protected override CommandOutput Execute(CommandContext context)
        {
            var obj = ConsoleAPI.Bindings.ResolveBinding(type, name, tag);
            return obj ? new CommandOutput($"Binding set:  {type}  =>  {obj}") : new CommandOutput("Binding could not be set.");
        }
    }

    [Subcommand("list", "Lists all current bindings.", typeof(BindCommand))]
    public class ListBindingsCommand : SimpleCommand
    {
        [Switch('t', "The type of the object you want to show.")]
        private Type type = null;

        protected override CommandOutput Execute(CommandContext context)
        {
            var bindings = ConsoleAPI.Bindings.GetAllBindings();
            if (bindings.Count == 0) return new CommandOutput("There are no bindings currently.");

            if (type == null)
            {
                List<string> lines = bindings.Select(kvp => $"{kvp.Key}  =>  {kvp.Value}").ToList();
                return new CommandOutput(string.Join(Environment.NewLine, lines));
            }

            if (!bindings.TryGetValue(type, out Object obj))
            {
                return new CommandOutput($"Binding for {type} not set.");
            }

            return new CommandOutput($"{type}  =>  {obj}");
        }
    }


    [Subcommand("remove", "Removes object bindings.", typeof(BindCommand))]
    public class RemoveBindingCommand : SimpleCommand
    {
        [Positional(0, "Which binding type to clear.")]
        private Type type;

        protected override CommandOutput Execute(CommandContext context)
        {
            var bindings = ConsoleAPI.Bindings.GetAllBindings();

            if (!bindings.Remove(type, out var obj))
            {
                return new CommandOutput($"No binding of type '{type}'.");
            }

            return new CommandOutput($"Binding '{type}  =>  {obj}' was removed.");
        }
    }

    [ConfirmBeforeExecuting("Are you sure you want to clear all bindings?")]
    [Subcommand("all", "Clears all bindings.", typeof(BindCommand))]
    public class RemoveAllBindingsCommand : SimpleCommand
    {
        protected override CommandOutput Execute(CommandContext context)
        {
            var bindings = ConsoleAPI.Bindings.GetAllBindings();
            if (bindings == null || bindings.Count == 0) return new CommandOutput("There are no bindings currently.");

            bindings.Clear();
            return new CommandOutput("All bindings cleared.");
        }
    }
}
