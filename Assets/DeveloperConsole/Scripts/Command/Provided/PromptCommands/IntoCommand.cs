using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command;

namespace DeveloperConsole
{
    [Command("into", "Runs future commands in the context of the first one until escaped.")]
    public class IntoCommand : PromptCommand
    {
        [Optional(0, "The command to go into")]
        private string parentCommand = null;

        [Variadic("Subcommands to go further into.")]
        private List<string> subcommands;

        protected override async Task<CommandOutput> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            if (parentCommand == null) return Help();

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
                        if (resolver.Tokens[0] == null || !resolver.Tokens[0].StartsWith("."))
                        {
                            resolver.Tokens.Insert(0, prefix);
                        }
                        else
                        {
                            resolver.Tokens[0] = resolver.Tokens[0][1..];
                        }
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

        private CommandOutput Help()
        {
            string message =
                $"Type {MessageFormatter.AddColor("into", MessageFormatter.Blue)} followed by and command to run " +
                "subsequent commands in the scope of the first. For example, " +
                $"{MessageFormatter.AddColor("into scene", MessageFormatter.Blue)} would allow the next command " +
                $"{MessageFormatter.AddColor("list", MessageFormatter.Blue)} to run as 'scene list'. " +
                $"You can also enter subcommands, {MessageFormatter.AddColor("into <parent> <child> ...", MessageFormatter.Blue)}." +
                $"{Environment.NewLine}{Environment.NewLine}" +
                "To run a command from the global registry while using into, use a '.' at the front of the command. " +
                $"For example, {MessageFormatter.AddColor(".clear", MessageFormatter.Blue)} will still clear the screen " +
                $"and {MessageFormatter.AddColor(".scene list", MessageFormatter.Blue)} will show the scene list." +
                $"{Environment.NewLine}{Environment.NewLine}Press {MessageFormatter.AddColor("ctrl + c", MessageFormatter.Red)}" +
                " to exit the command loop.";

            return new CommandOutput(message);
        }
    }
}
