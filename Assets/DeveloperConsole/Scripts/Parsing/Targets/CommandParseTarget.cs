using System;
using System.Collections.Generic;
using DeveloperConsole.Command;

namespace DeveloperConsole.Parsing
{
    public class CommandParseTarget : IParseTarget
    {
        /// <summary>
        /// The command being constructed.
        /// </summary>
        public ICommand Command { get; }
        
        private CommandSchema Schema { get; }
        
        /// <summary>
        /// Creates a new command parse target.
        /// </summary>
        /// <param name="schema">The schema to build.</param>
        public CommandParseTarget(CommandSchema schema)
        {
            Schema = schema;
            Command = Activator.CreateInstance(schema.CommandType) as ICommand;
        }
        
        public HashSet<ArgumentSpecification> GetArguments()
        {
            return Schema.ArgumentSpecifications;
        }

        public void SetArgument(ArgumentSpecification argument, object argValue)
        {
            argument.FieldInfo.SetValue(Command, argValue);
        }
    }
}