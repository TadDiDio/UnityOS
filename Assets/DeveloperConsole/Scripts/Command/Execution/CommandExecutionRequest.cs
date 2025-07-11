using DeveloperConsole.Core.Shell;
using DeveloperConsole.IO;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// A request for a command to be executed.
    /// </summary>
    public class CommandExecutionRequest
    {
        /// <summary>
        /// The session submitting this.
        /// </summary>
        public ShellSession ShellSession;


        /// <summary>
        /// The input to be run.
        /// </summary>
        public ICommandInput Input;

        /// <summary>
        /// The execution shell.
        /// </summary>
        public ShellApplication Shell;
    }
}
