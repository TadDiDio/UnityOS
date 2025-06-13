using DeveloperConsole.Core;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// A request to execute a command.
    /// </summary>
    public class CommandRequest
    {
        /// <summary>
        /// The session requesting this command.
        /// </summary>
        public ShellSession ShellSession { get; }
        
        
        /// <summary>
        /// The resolver for this request.
        /// </summary>
        public ICommandResolver CommandResolver { get; }

        
        /// <summary>
        /// Creates a command request.
        /// </summary>
        /// <param name="session">The session requesting this command.</param>
        /// <param name="commandResolver">A command resolver giving a command.</param>
        public CommandRequest(ShellSession session, ICommandResolver commandResolver)
        {
            ShellSession = session;
            CommandResolver = commandResolver;
        }
    }
}