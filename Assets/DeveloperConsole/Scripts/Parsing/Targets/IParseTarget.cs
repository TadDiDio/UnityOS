using System.Collections.Generic;
using DeveloperConsole.Command;

namespace DeveloperConsole.Parsing
{
    /// <summary>
    /// A target for parses to parse into.
    /// </summary>
    public interface IParseTarget
    {
        /// <summary>
        /// All arguments that this target can receive.
        /// </summary>
        /// <returns>A set of arguments.</returns>
        public HashSet<ArgumentSpecification> GetArguments();


        /// <summary>
        /// Gets the first arg spec and value matching the attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns>The arg spec and value.</returns>
        public (ArgumentSpecification spec, object value)? GetFirstArgumentMatchingAttribute(
            ArgumentAttribute attribute);
        
        
        /// <summary>
        /// Sets an argument to a given value.
        /// </summary>
        /// <param name="argument">The argument to set.</param>
        /// <param name="argValue">The value to set it to.</param>
        public void SetArgument(ArgumentSpecification argument, object argValue);
    }
}