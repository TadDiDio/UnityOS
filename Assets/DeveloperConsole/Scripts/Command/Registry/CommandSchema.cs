using System;
using System.Collections.Generic;
using System.Linq;
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
        public readonly string Name;
        
        /// <summary>
        /// The description of the arg.
        /// </summary>
        public string Description;
        
        // The field representing this arg.
        public readonly FieldInfo FieldInfo;
        
        /// <summary>
        /// A list of all attributes on this argument.
        /// </summary>
        public readonly IReadOnlyList<ArgumentAttribute> Attributes;

        /// <summary>
        /// Creates a new argument specification based on a field. If the field does not
        /// have an argument attribute it will not be initialized.
        /// </summary>
        /// <param name="fieldInfo">The field.</param>
        public ArgumentSpecification(FieldInfo fieldInfo)
        {
            Attributes = fieldInfo.GetCustomAttributes<ArgumentAttribute>().ToList();
            if (Attributes == null || Attributes.Count == 0) return;

            FieldInfo = fieldInfo;
            Name = Attributes[0].Name ?? fieldInfo.Name;
            Description = Attributes[0].Description;
        }
    }
}