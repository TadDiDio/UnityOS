using DeveloperConsole.Core;
using DeveloperConsole.Command;

namespace DeveloperConsole.IO
{
    /// <summary>
    /// A fully constructed command as input.
    /// </summary>
    public class CommandInput : IInput
    {
        public ShellSession ShellSession { get; }

        private ICommand _command;
        
        
        /// <summary>
        /// Creates a new command input.
        /// </summary>
        /// <param name="session">The associated shell session.</param>
        /// <param name="command">The command to input.</param>
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