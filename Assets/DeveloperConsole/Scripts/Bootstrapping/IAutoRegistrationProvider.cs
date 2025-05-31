using System;
using System.Collections.Generic;

namespace DeveloperConsole
{
    public interface IAutoRegistrationProvider
    {
        public Dictionary<string, Type> AllCommands(IAutoRegistrationStrategy strategy, int maxParseDepth = 10);
    }
}