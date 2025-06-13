using DeveloperConsole.Command;
using DeveloperConsole.Core;

namespace DeveloperConsole.IO
{
    public interface IInput
    {
        public ShellSession ShellSession { get; }
        public CommandRequest GetCommandRequest();
    }
}