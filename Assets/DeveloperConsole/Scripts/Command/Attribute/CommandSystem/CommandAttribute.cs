using System;
using System.Reflection;
using UnityEditorInternal.VersionControl;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Marks a class as a command for registration and gives a name and description.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        private string _rawName;
        private string _rawDescription;
        private Type _parentType;

        /// <summary>
        /// This command's name.
        /// </summary>
        public string Name => CommandMetaProcessor.Name(_rawName);


        /// <summary>
        /// This command's description.
        /// </summary>
        public string Description => CommandMetaProcessor.Description(_rawDescription);


        /// <summary>
        /// Defines a command.
        /// </summary>
        /// <param name="name">The invocation word for this command.</param>
        /// <param name="description">What this command does.</param>
        /// <param name="parentType">The type of the parent.</param>
        public CommandAttribute(string name, string description, Type parentType = null)
        {
            _rawName = name;
            _rawDescription = description;
            _parentType = parentType;
        }

        /// <summary>
        /// Gets the parent type of this command, or null if there is none.
        /// </summary>
        /// <param name="self">The type of this command.</param>
        /// <returns>The parent type or null.</returns>
        public Type GetParentType(Type self)
        {
            if (_parentType is not null) return _parentType;
            return self.DeclaringType?.GetCustomAttribute<CommandAttribute>() is not null ? self.DeclaringType : null;
        }
    }
}
