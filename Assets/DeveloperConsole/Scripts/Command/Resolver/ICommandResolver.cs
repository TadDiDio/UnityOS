using System.Collections.Generic;
using DeveloperConsole.Core.Shell;

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
        /// <param name="expandAliases">Tells whether to expand aliases.</param>
        /// <returns>The result.</returns>
        public CommandResolutionResult Resolve(ShellSession session, bool expandAliases);
    }

    public enum CommandResolutionStatus
    {
        Success,
        Fail,
        AliasExpansion
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
        public CommandResolutionStatus Status;


        /// <summary>
        /// Tokens to reevaluate when an alias expansion occured.
        /// </summary>
        public List<string> Tokens;


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
                Status = CommandResolutionStatus.Fail,
                ErrorMessage = message,
                Tokens = null
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
                Status = CommandResolutionStatus.Success,
                ErrorMessage = string.Empty,
                Tokens = null
            };
        }

        public static CommandResolutionResult AliasExpansion(List<string> tokens)
        {
            return new CommandResolutionResult
            {
                Command = null,
                Status = CommandResolutionStatus.AliasExpansion,
                ErrorMessage = string.Empty,
                Tokens = tokens
            };
        }
    }
}
