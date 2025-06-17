using DeveloperConsole.Core;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Interface for command resolvers.
    /// </summary>
    public interface ICommandResolver
    {
        /// <summary>
        /// Resolves to a command.
        /// </summary>
        /// <param name="session">The session executing this command.</param>
        /// <returns>The result.</returns>
        public CommandResolutionResult Resolve(IShellSession session);
    }

    
    /// <summary>
    /// Holds the results of a command resolution.
    /// </summary>
    public class CommandResolutionResult
    {
        /// <summary>
        /// The command if successful.
        /// </summary>
        public ICommand Command;
        
        /// <summary>
        /// The error message if failed.
        /// </summary>
        public string ErrorMessage;
        
        /// <summary>
        /// The failure status.
        /// </summary>
        public Status Status;

        
        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="message">The failure message.</param>
        /// <returns>The result.</returns>
        public static CommandResolutionResult Failed(string message)
        {
            return new CommandResolutionResult
            {
                Command = null,
                Status = Status.Fail,
                ErrorMessage = message
            };
        }

        
        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <param name="command">The resolved command.</param>
        /// <returns>The result.</returns>
        public static CommandResolutionResult Success(ICommand command)
        {
            return new CommandResolutionResult
            {
                Command = command,
                Status = Status.Success,
                ErrorMessage = string.Empty
            };
        }
    }
}