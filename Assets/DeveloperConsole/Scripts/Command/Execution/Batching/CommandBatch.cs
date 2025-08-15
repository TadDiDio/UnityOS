using System.Collections.Generic;

namespace DeveloperConsole.Command
{
    public class CommandBatch
    {
        public bool AllowPrompting; // TODO: This will be replaced with a filter.
        public readonly List<CommandRequest> Requests = new();
    }
}
