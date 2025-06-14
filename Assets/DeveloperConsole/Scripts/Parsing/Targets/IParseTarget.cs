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
        /// Sets an argument to a given value.
        /// </summary>
        /// <param name="argument">The argument to set.</param>
        /// <param name="argValue">The value to set it to.</param>
        public void SetArgument(ArgumentSpecification argument, object argValue);
    }
}