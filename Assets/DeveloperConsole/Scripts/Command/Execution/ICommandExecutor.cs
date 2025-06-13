using System.Threading.Tasks;

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
        /// <returns>The execution result.</returns>
        public Task<CommandExecutionResult> ExecuteCommand(CommandExecutionRequest executionRequest);
    }

    
    /// <summary>
    /// The result of trying to execute a command.
    /// </summary>
    public class CommandExecutionResult
    {
        /// <summary>
        /// Whether it succeeded.
        /// </summary>
        public Status Status;
        
        
        /// <summary>
        /// An error message if there was one.
        /// </summary>
        public string ErrorMessage;
        
        
        /// <summary>
        /// The output from the command.
        /// </summary>
        public CommandOutput CommandOutput;

        
        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>The result.</returns>
        public static CommandExecutionResult Fail(string errorMessage)
        {
            return new CommandExecutionResult
            {
                Status = Status.Fail,
                ErrorMessage = errorMessage,
                CommandOutput = null
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
                Status = Status.Success,
                ErrorMessage = string.Empty,
                CommandOutput = output
            };
        }
    }
}