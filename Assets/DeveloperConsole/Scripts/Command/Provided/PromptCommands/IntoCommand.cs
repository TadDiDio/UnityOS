using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command;

namespace DeveloperConsole
{
    [Command("into", "Runs future commands in the context of the first one until escaped.")]
    public class IntoCommand : PromptCommand
    {
        [Positional(0, "The command to go into ")]
        private string parentCommand;

        [Variadic("Subcommands to go further into.")]
        private List<string> subcommands;

        protected override async Task<CommandOutput> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            subcommands.Insert(0, parentCommand);
            string prefix = string.Join(".", subcommands);

            if (!ConsoleAPI.Commands.TryResolveCommandSchema(prefix, out _))
            {
                return new CommandOutput($"Could not resolve a command for {prefix}", Status.Fail);
            }

            try
            {
                Session.PushPromptPrefix(CommandId, prefix);

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var batch = await PromptForCommand(cancellationToken);

                    foreach (var request in batch.Requests)
                    {
                        if (request.Resolver is not TokenCommandResolver resolver) continue;
                        resolver.Tokens.Insert(0, prefix);
                    }

                    await context.Session.SubmitBatch(batch, Session.GetInterface(), cancellationToken);
                }
            }
            finally
            {
                Session.PopPromptPrefix(CommandId);
            }

            // ReSharper disable once FunctionNeverReturns
        }
    }
}
