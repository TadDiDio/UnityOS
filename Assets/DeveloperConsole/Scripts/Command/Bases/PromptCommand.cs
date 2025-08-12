using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Core.Shell;
using DeveloperConsole.Parsing.TypeAdapting.Types;

namespace DeveloperConsole.Command
{
    public abstract class PromptCommand : AsyncCommand
    {
        /// <summary>
        /// Prompts the user for a confirmation.
        /// </summary>
        /// <param name="message">A message to display.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>True if the confirmation passed.</returns>
        protected async Task<bool> ConfirmAsync(string message, CancellationToken cancellationToken)
        {
            var prompt = Prompt.Confirmation(message);
            var result = await Session.PromptAsync<ConfirmationResult>(CommandId, prompt, cancellationToken);
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
            return await Session.PromptAsync<CommandBatch>(CommandId, prompt, cancellationToken);
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
            return await Session.PromptAsync<T>(CommandId, prompt, cancellationToken);
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
            return await Session.PromptAsync<T>(CommandId, prompt, cancellationToken);
        }
    }
}
