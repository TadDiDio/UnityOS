using DeveloperConsole.Core;
using DeveloperConsole.Command;

namespace DeveloperConsole.IO
{
    public class ShellOutputMessage : IOutputMessage
    {
        // TODO: Every IOutput message should have a channel and session for routing purposes
        public string Channel;
        public ShellSession Session;
        public CommandOutput CommandOutput;
    }
}