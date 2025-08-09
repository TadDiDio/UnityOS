using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Core.Shell;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Interface for command executors.
    /// </summary>
    public interface ICommandExecutor
    {
        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="executionRequest">The request.</param>
        /// <param name="userInterface">The user interface this execution should use.</param>
        /// <param name="cancellationToken">The command's cancellation token.</param>
        /// <returns>The execution result.</returns>
        public Task<CommandExecutionResult> ExecuteCommand(ShellRequest executionRequest,
            UserInterface userInterface,
            CancellationToken cancellationToken);
    }


    /// <summary>
    /// The result of trying to execute a command.
    /// </summary>
    public class CommandExecutionResult
    {
        /// <summary>
        /// Whether it succeeded.
        /// </summary>
        public CommandResolutionStatus Status;

        /// <summary>
        /// Updated token list when performing alias expansion.
        /// </summary>
        public List<string> Tokens;

        /// <summary>
        /// Tells if the error message is valid.
        /// </summary>
        public bool ErrorMessageValid;

        /// <summary>
        /// An error message.
        /// </summary>
        public string ErrorMessage;

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <returns>The result.</returns>
        public static CommandExecutionResult Fail(string errorMessage = null)
        {
            return new CommandExecutionResult
            {
                Status = CommandResolutionStatus.Fail,
                Tokens = null,
                ErrorMessageValid = errorMessage != null,
                ErrorMessage = errorMessage
            };
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <returns>The result.</returns>
        public static CommandExecutionResult Success()
        {
            return new CommandExecutionResult
            {
                Status = CommandResolutionStatus.Success,
                Tokens = null
            };
        }


        /// <summary>
        /// Creates an alias expansion result.
        /// </summary>
        /// <param name="tokens">All tokens including the expanded alias.</param>
        /// <returns>The result.</returns>
        public static CommandExecutionResult AliasExpansion(List<string> tokens)
        {
            return new CommandExecutionResult
            {
                Status = CommandResolutionStatus.AliasExpansion,
                Tokens = tokens
            };
        }
    }
}
