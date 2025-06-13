using System;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Base class for all argument attributes. Used for discoverability.
    /// </summary>
    public abstract class ArgumentAttribute : Attribute
    {
        /// <summary>
        /// The override for an argument name. If not set, the field's name will be used.
        /// </summary>
        public string Name;
        
        
        /// <summary>
        /// The argument's description.
        /// </summary>
        public string Description => CommandMetaProcessor.Description(_rawDescription);
        

        private string _rawDescription;
        protected ArgumentAttribute(string description, string overrideName = null)
        {
            Name = overrideName;
            _rawDescription = description;
        }
    }
}