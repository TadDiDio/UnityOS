using DeveloperConsole.Core;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Wrapper to resolve a command to a command. Not a hard task :P
    /// </summary>
    public class CommandCommandResolver : ICommandResolver
    {
        private ICommand _command;
        
        /// <summary>
        /// Creates a new command command resolver.
        /// </summary>
        /// <param name="command">The command to resolve to.</param>
        public CommandCommandResolver(ICommand command)
        {
            _command = command;
        }
        
        public CommandResolutionResult Resolve(IShellSession session) => CommandResolutionResult.Success(_command);
    }
}