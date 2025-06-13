using System;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Declares a positional arg.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class PositionalAttribute : ArgumentAttribute
    {
        /// <summary>
        /// The index in the command.
        /// </summary>
        public int Index { get; }
        
        
        /// <summary>
        /// Creates a positional arg.
        /// </summary>
        /// <param name="index">The index this arg must appear.</param>
        /// <param name="description">The arg's description.</param>
        /// <param name="overrideName">Overrides the field name.</param>
        public PositionalAttribute(int index, string description, string overrideName = null) 
            : base(description, overrideName)
        {
            Index = index;
        }
    }
}