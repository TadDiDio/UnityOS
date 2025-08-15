using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Command interface.
    /// </summary>
    public interface ICommand : IDisposable
    {
        /// <summary>
        /// The schema representing this command.
        /// </summary>
        public CommandSchema Schema { get; }

        /// <summary>
        /// Defines the asynchronous execution logic for this command.
        /// </summary>
        /// <param name="context">The context of this execution.</param>
        /// <param name="cancellationToken">A cancellation token to stop execution.</param>
        /// <returns>The command's output.</returns>
        public Task<CommandOutput> ExecuteCommandAsync(FullCommandContext context, CancellationToken cancellationToken);

        /// <summary>
        /// Instantiates and initializes a new command.
        /// </summary>
        /// <param name="schema">The schema to build.</param>
        /// <returns>The command.</returns>
        public static ICommand Create(CommandSchema schema)
        {
            var command = (CommandBase)Activator.CreateInstance(schema.CommandType);
            command.Initialize(schema);
            return command;
        }
    }
}
