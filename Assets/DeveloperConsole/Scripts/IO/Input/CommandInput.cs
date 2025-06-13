using DeveloperConsole.Core;
using DeveloperConsole.Command;

namespace DeveloperConsole.IO
{
    public class CommandInput : IInput
    {
        public ShellSession ShellSession { get; }

        private ICommand _command;
        
        public CommandInput(ShellSession session, ICommand command)
        {
            ShellSession = session;
            _command = command;
        }

        public CommandRequest GetCommandRequest()
        {
            return new CommandRequest(ShellSession, new CommandCommandResolver(_command));
        }
    }
}