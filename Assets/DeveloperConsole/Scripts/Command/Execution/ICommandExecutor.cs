using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Core.Shell;
using NUnit.Framework;

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
        /// <param name="cancellationToken">The command's cancellation token.</param>
        /// <returns>The execution result.</returns>
        public Task<CommandExecutionResult> ExecuteCommand(ShellRequest executionRequest, CancellationToken cancellationToken);
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
        /// An error message if there was one.
        /// </summary>
        public string ErrorMessage;

        /// <summary>
        /// The output from the command.
        /// </summary>
        public CommandOutput CommandOutput;

        /// <summary>
        /// Updated token list when performing alias expansion.
        /// </summary>
        public List<string> Tokens;

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>The result.</returns>
        public static CommandExecutionResult Fail(string errorMessage)
        {
            return new CommandExecutionResult
            {
                Status = CommandResolutionStatus.Fail,
                ErrorMessage = errorMessage,
                CommandOutput = null,
                Tokens = null
            };
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <param name="output">The command output.</param>
        /// <returns>The result.</returns>
        public static CommandExecutionResult Success(CommandOutput output)
        {
            return new CommandExecutionResult
            {
                Status = CommandResolutionStatus.Success,
                ErrorMessage = string.Empty,
                CommandOutput = output,
                Tokens = null
            };
        }


        public static CommandExecutionResult AliasExpansion(List<string> tokens)
        {
            return new CommandExecutionResult
            {
                Status = CommandResolutionStatus.AliasExpansion,
                ErrorMessage = string.Empty,
                CommandOutput = null,
                Tokens = tokens
            };
        }
    }
}
