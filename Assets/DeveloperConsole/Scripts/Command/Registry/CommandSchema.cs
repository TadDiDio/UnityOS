using System;
using System.Collections.Generic;
using System.Reflection;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Defines the way a command will be constructed and executed.
    /// </summary>
    public class CommandSchema
    {
        /// <summary>
        /// The command name.
        /// </summary>
        public string Name;
        
        /// <summary>
        /// The command description.
        /// </summary>
        public string Description;
        
        /// <summary>
        /// The command type.
        /// </summary>
        public Type CommandType;
        
        /// <summary>
        /// The parent of this command if it has one.
        /// </summary>
        public CommandSchema ParentSchema;
        
        /// <summary>
        /// A set of all subcommands.
        /// </summary>
        public HashSet<CommandSchema> Subcommands;
        
        /// <summary>
        /// A set of all args this command takes.
        /// </summary>
        public HashSet<ArgumentSpecification> ArgumentSpecifications;
    }
    

    /// <summary>
    /// Defines how a command parameter will behave.
    /// </summary>
    public class ArgumentSpecification
    {
        /// <summary>
        /// The name of the arg.
        /// </summary>
        public string Name;
        
        /// <summary>
        /// The description of the arg.
        /// </summary>
        public string Description;
        
        // The field representing this arg.
        public FieldInfo FieldInfo;
        
        /// <summary>
        /// A list of all attributes on this argument.
        /// </summary>
        public IReadOnlyList<ArgumentAttribute> Attributes;
    }
}