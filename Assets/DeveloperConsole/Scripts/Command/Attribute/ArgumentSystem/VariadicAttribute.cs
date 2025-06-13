using System;
using System.Collections.Generic;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Creates new variadic args.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class VariadicAttribute : Validated
    {
        /// <summary>
        /// Whether these variadic args represent a command path.
        /// </summary>
        public bool IsCommandPath;
     
        private string _commandPath;
        
        
        /// <summary>
        /// Creates a new variadic arg list.
        /// </summary>
        /// <param name="description">The arg's description.</param>
        /// <param name="overrideName">Overrides the field name.</param>
        /// <param name="isCommandPath">Whether this is a command path.</param>
        public VariadicAttribute(string description, string overrideName = null, bool isCommandPath = false)
            : base(description, overrideName)
        {
            IsCommandPath = isCommandPath;
        }

        protected override bool Validate(AttributeValidationData data)
        {
            if (!IsCommandPath) return true;

            if (data.FieldInfo.GetValue(data.Object) is not List<string> path) return false;
            _commandPath = string.Join(".", path);
            
            return ConsoleAPI.Commands.IsValidCommand(_commandPath);
        }

        public override string ErrorMessage() => $"{_commandPath} is not a valid command.";
    }
}