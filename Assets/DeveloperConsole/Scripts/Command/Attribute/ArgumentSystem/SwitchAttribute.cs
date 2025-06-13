using System;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Defines a switch arg.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SwitchAttribute : ArgumentAttribute
    {
        public string ShortName { get; }
        
        
        /// <summary>
        /// Creates a new switch arg.
        /// </summary>
        /// <param name="shortName">The short name for this switch.</param>
        /// <param name="description">The arg's description.</param>
        /// <param name="overrideName">Overrides the field name.</param>
        public SwitchAttribute(string shortName, string description, string overrideName = null) 
            : base(description, overrideName)
        {
            ShortName = shortName;
        }
    }
}