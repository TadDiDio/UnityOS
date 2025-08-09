using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Core.Shell;
using DeveloperConsole.Parsing.TypeAdapting.Types;

namespace DeveloperConsole.Command
{
    public abstract class PromptCommand : AsyncCommand
    {
        protected async Task<bool> ConfirmAsync(string message, CancellationToken cancellationToken)
        {
            var prompt = Prompt.Confirmation(message);
            var result = await Session.PromptAsync<ConfirmationResult>(CommandId, prompt, cancellationToken);
            return result.Success;
        }

        protected async Task<T> PromptAsync<T>(string message, CancellationToken cancellationToken)
        {
            var prompt = Prompt.General<T>(message);
            return await Session.PromptAsync<T>(CommandId, prompt, cancellationToken);
        }

        protected async Task<T> PromptWithChoicesAsync<T>(string message, PromptChoice[] choices, CancellationToken cancellationToken)
        {
            var prompt = Prompt.Choice<T>(message, choices);
            return await Session.PromptAsync<T>(CommandId, prompt, cancellationToken);
        }
    }
}
