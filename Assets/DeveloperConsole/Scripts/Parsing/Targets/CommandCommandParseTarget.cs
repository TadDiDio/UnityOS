using System;
using System.Collections.Generic;
using DeveloperConsole.Command;

namespace DeveloperConsole.Parsing
{
    public class CommandCommandParseTarget : AttributeValidatedCommandParseTarget
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
        public CommandCommandParseTarget(CommandSchema schema)
        {
            Schema = schema;
            Command = Activator.CreateInstance(schema.CommandType) as ICommand;
        }
        
        public override HashSet<ArgumentSpecification> GetArguments() => Schema.ArgumentSpecifications;


        protected override void SetArgumentValue(ArgumentSpecification argument, object argValue)
        {
            argument.FieldInfo.SetValue(Command, argValue);
        }
    }
}