using DeveloperConsole.Core.Shell;

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
        /// The command resolver for this input.
        /// </summary>
        public ICommandResolver Resolver;

        /// <summary>
        /// The execution shell.
        /// </summary>
        public ShellApplication Shell;
    }
}
