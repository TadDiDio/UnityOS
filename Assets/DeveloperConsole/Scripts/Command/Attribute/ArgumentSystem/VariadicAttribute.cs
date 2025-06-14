using System;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Creates new variadic args.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class VariadicAttribute : ArgumentAttribute
    {
        /// <summary>
        /// Creates a new variadic arg list.
        /// </summary>
        /// <param name="description">The arg's description.</param>
        /// <param name="overrideName">Overrides the field name.</param>
        public VariadicAttribute(string description, string overrideName = null) : base(description, overrideName) { }
    }
}