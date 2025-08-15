using System;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Core.Shell;
using DeveloperConsole.Parsing.TypeAdapting.Types;
using DeveloperConsole.Scripts.Utils;

namespace DeveloperConsole.Command
{
    public abstract class AsyncCommand : CommandBase
    {
        private FullCommandContext _context;
        protected override async Task<CommandOutput> ExecuteAsync(FullCommandContext fullContext, CancellationToken cancellationToken)
        {
            _context = fullContext;
            return await Execute(new AsyncCommandContext(_context.Environment), cancellationToken);
        }

        /// <summary>
        /// Executes this command.
        /// </summary>
        /// <param name="context">The context and permissions this command has.</param>
        /// <param name="cancellationToken">The </param>
        /// <returns></returns>
        protected abstract Task<CommandOutput> Execute(AsyncCommandContext context, CancellationToken cancellationToken);


        /// <summary>
        /// Prompts the user for a confirmation.
        /// </summary>
        /// <param name="message">A message to display.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>True if the confirmation passed.</returns>
        protected async Task<bool> ConfirmAsync(string message, CancellationToken cancellationToken)
        {
            var prompt = Prompt.Confirmation(message);
            var result = await _context.Prompting.PromptAsync<ConfirmationResult>(prompt, cancellationToken);
            return result.Success;
        }

        /// <summary>
        /// Prompts the user to input a command.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A command batch to execute.</returns>
        protected async Task<CommandBatch> PromptForCommand(CancellationToken cancellationToken)
        {
            var prompt = Prompt.Command();
            return await _context.Prompting.PromptAsync<CommandBatch>(prompt, cancellationToken);
        }

        /// <summary>
        /// Prompts for a general type.
        /// </summary>
        /// <param name="message">A message to display.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <typeparam name="T">The type of response to ask for.</typeparam>
        /// <returns>The response.</returns>
        protected async Task<T> PromptAsync<T>(string message, CancellationToken cancellationToken)
        {
            var prompt = Prompt.General<T>(message);
            return await _context.Prompting.PromptAsync<T>(prompt, cancellationToken);
        }

        /// <summary>
        /// Prompts with a set range of choices.
        /// </summary>
        /// <param name="message">A message to display.</param>
        /// <param name="choices">The array of choices to limit answers to.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <typeparam name="T">The type of the response to ask for.</typeparam>
        /// <returns>The response.</returns>
        protected async Task<T> PromptWithChoicesAsync<T>(string message, PromptChoice[] choices, CancellationToken cancellationToken)
        {
            var prompt = Prompt.Choice<T>(message, choices);
            return await _context.Prompting.PromptAsync<T>(prompt, cancellationToken);
        }

        /// <summary>
        /// Call this method in a using tag to push a prompt header for the duration.
        /// </summary>
        /// <param name="prefix">The prefix to add.</param>
        protected IDisposable PushPromptForScope(string prefix)
        {
            return _context.Prompting.PushPromptPrefixScope(prefix);
        }

        /// <summary>
        /// Runs a command batch in this session.
        /// </summary>
        /// <param name="commandBatch">The batch.</param>
        /// <param name="cancellationToken">The token used to cancel these commands.</param>
        /// <param name="ioContext">An override ioContext. If null, the default will be used.</param>
        /// <returns>The command output.</returns>
        protected async Task RunCommandBatch(CommandBatch commandBatch, CancellationToken cancellationToken, IOContext ioContext = null)
        {
            await _context.Session.CommandSubmitter.SubmitBatch(commandBatch, cancellationToken);
        }
    }
}
