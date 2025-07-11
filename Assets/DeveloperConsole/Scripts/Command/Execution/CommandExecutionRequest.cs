using DeveloperConsole.Core.Shell;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// A request for a command to be executed.
    /// </summary>
    public class CommandExecutionRequest
    {
        /// <summary>
        /// The requested command and it's context.
        /// </summary>
        public CommandRequest Request;
        
        /// <summary>
        /// The execution shell.
        /// </summary>
        public ShellApplication Shell;
    }
}