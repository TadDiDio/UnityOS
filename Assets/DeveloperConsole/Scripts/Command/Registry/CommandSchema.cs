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
            var info = Attributes.OfType<InformativeAttribute>().FirstOrDefault();
            
            FieldInfo = fieldInfo;
            Name = info?.Name ?? fieldInfo.Name;
            Description = info?.Description ?? "Missing description.";
        }

        
        /// <summary>
        /// Gets all argument specifications from a type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>A set of specs.</returns>
        public static HashSet<ArgumentSpecification> GetAllFromType<T>()
        {
            return GetAllFromType(typeof(T));
        }
        
        
        /// <summary>
        /// Gets all argument specifications from a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A set of specs.</returns>
        public static HashSet<ArgumentSpecification> GetAllFromType(Type type)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            var allFields = type.GetFields(flags);

            HashSet<ArgumentSpecification> specs = new();
            
            foreach (var field in allFields)
            {
                var attributes = field.GetCustomAttributes<ArgumentAttribute>().ToList();
                if (attributes.Count == 0) continue;

                specs.Add(new ArgumentSpecification(field));
            }
            
            return specs;
        }
    }
}