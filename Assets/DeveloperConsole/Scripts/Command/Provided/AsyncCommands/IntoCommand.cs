using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command;

namespace DeveloperConsole
{
    [Command("into", "Runs future commands in the context of the first one until escaped.")]
    public class IntoCommand : AsyncCommand
    {
        [Optional(0, "The command to go into")]
        private string parentCommand = null;

        [Variadic("Subcommands to go further into.")]
        private List<string> subcommands;

        protected override async Task<CommandOutput> Execute(AsyncCommandContext context, CancellationToken cancellationToken)
        {
            if (parentCommand == null) return Help();

            subcommands.Insert(0, parentCommand);
            string prefix = string.Join(".", subcommands);

            if (!ConsoleAPI.Commands.TryResolveCommandSchema(prefix, out _))
            {
                return new CommandOutput($"Could not resolve a command for {prefix}", Status.Fail);
            }

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

                // await context.Session.SubmitBatch(batch, Session.GetInterface(), cancellationToken);
            }

            // ReSharper disable once FunctionNeverReturns
        }

        private CommandOutput Help()
        {
            string message =
                $"Type {Formatter.AddColor("into", Formatter.Blue)} followed by and command to run " +
                "subsequent commands in the scope of the first. For example, " +
                $"{Formatter.AddColor("into scene", Formatter.Blue)} would allow the next command " +
                $"{Formatter.AddColor("list", Formatter.Blue)} to run as 'scene list'. " +
                $"You can also enter subcommands, {Formatter.AddColor("into <parent> <child> ...", Formatter.Blue)}." +
                $"{Environment.NewLine}{Environment.NewLine}" +
                "To run a command from the global registry while using into, use a '.' at the front of the command. " +
                $"For example, {Formatter.AddColor(".clear", Formatter.Blue)} will still clear the screen " +
                $"and {Formatter.AddColor(".scene list", Formatter.Blue)} will show the scene list." +
                $"{Environment.NewLine}{Environment.NewLine}Press {Formatter.AddColor("ctrl + c", Formatter.Red)}" +
                " to exit the command loop.";

            return new CommandOutput(message);
        }
    }
}
