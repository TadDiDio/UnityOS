using System;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Marks a command as a subcommand.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SubcommandAttribute : CommandAttribute
    {
        /// <summary>
        /// The type of the parent command.
        /// </summary>
        public Type ParentCommandType { get; }
        
        
        /// <summary>
        /// Marks a class as a subcommand.
        /// </summary>
        /// <param name="name">The name of this subcommand.</param>
        /// <param name="description">What this subcommand does.</param>
        /// <param name="parentType">The type of the parent command.</param>
        public SubcommandAttribute(string name, string description, Type parentType) : base(name, description)
        {
            ParentCommandType = parentType;
        }
    }
}