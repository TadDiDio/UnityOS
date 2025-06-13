using System;

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
        public CommandAttribute(string name, string description)
        {
            _rawName = name;
            _rawDescription = description;
        }
    }

}