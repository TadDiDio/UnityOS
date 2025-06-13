using System.Collections.Generic;
using DeveloperConsole.Command;

namespace DeveloperConsole.Parsing
{
    public interface IParseTarget
    {
        public HashSet<ArgumentSpecification> GetArguments();
        public void SetArgument(ArgumentSpecification argument, object argValue);
    }
}