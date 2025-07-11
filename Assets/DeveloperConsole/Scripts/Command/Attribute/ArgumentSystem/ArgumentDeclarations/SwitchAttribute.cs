using System;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Defines a switch arg.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SwitchAttribute : ArgumentDeclarationAttribute
    {
        public char Alias { get; }


        /// <summary>
        /// Creates a new switch arg.
        /// </summary>
        /// <param name="alias">The short name for this switch.</param>
        /// <param name="description">The arg's description.</param>
        /// <param name="overrideName">Overrides the field name.</param>
        public SwitchAttribute(char alias, string description, string overrideName = null)
            : base(description, overrideName)
        {
            Alias = alias;
        }

        public override string ToString()
        {
            return $"{Name ?? "Switch"} ({Alias}): {Description}";
        }
    }
}
