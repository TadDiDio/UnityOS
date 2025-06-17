using DeveloperConsole.Core;
using DeveloperConsole.Command;

namespace DeveloperConsole.IO
{
    /// <summary>
    /// An output message representing shell output.
    /// </summary>
    public class ShellOutputMessage : IOutputMessage
    {
        public IShellSession Session { get; }
        
        
        /// <summary>
        /// The output of the executed command.
        /// </summary>
        public CommandOutput CommandOutput;


        /// <summary>
        /// Creates a new shell output message.
        /// </summary>
        /// <param name="session">The session this output came from.</param>
        /// <param name="commandOutput">The output of the executed command.</param>
        public ShellOutputMessage(IShellSession session, CommandOutput commandOutput)
        {
            Session = session;
            CommandOutput = commandOutput;
        }
    }
}