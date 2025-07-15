using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Core.Shell;
using DeveloperConsole.Parsing.TypeAdapting.Types;

namespace DeveloperConsole.Command
{
    public abstract class PromptCommand : AsyncCommand
    {
        private ShellSession _session;
        public override async Task<CommandOutput> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            _session = context.Session;

            return await Execute(context, cancellationToken);
        }

        protected abstract Task<CommandOutput> Execute(CommandContext context, CancellationToken cancellationToken);

        protected async Task<bool> ConfirmAsync(string message, CancellationToken cancellationToken)
        {
            var prompt = Prompt.Confirmation(message);
            var result = await _session.PromptAsync<ConfirmationResult>(prompt, cancellationToken);
            return result.Success;
        }

        protected async Task<T> PromptAsync<T>(string message, CancellationToken cancellationToken)
        {
            var prompt = Prompt.General<T>(message);
            return await _session.PromptAsync<T>(prompt, cancellationToken);
        }

        protected async Task<T> PromptWithChoicesAsync<T>(string message, PromptChoice[] choices, CancellationToken cancellationToken)
        {
            var prompt = Prompt.Choice<T>(message, choices);
            return await _session.PromptAsync<T>(prompt, cancellationToken);
        }
    }
}
