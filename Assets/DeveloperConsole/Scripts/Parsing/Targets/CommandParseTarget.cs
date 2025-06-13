using System;
using System.Collections.Generic;
using DeveloperConsole.Command;

namespace DeveloperConsole.Parsing
{
    public class CommandParseTarget : IParseTarget
    {
        public ICommand Command { get; }
        public CommandSchema Schema { get; }
        
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