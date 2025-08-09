using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command;
using DeveloperConsole.Core.Shell;
using DeveloperConsole.IO;
using Task = System.Threading.Tasks.Task;

namespace DeveloperConsole
{
    [Command("watch", "Runs a command repeatedly.")]
    public class WatchCommand : AsyncCommand, IPromptResponder, IOutputChannel
    {
        [Switch('i', "The interval to repeat on.")]
        private float intervalSeconds = 1.0f;

        [Switch('o', "Whether to overwrite output.")]
        private bool overwrite = true;

        [Switch('k', "Whether to kill the loop if the command errors.")]
        private bool killOnError = true;

        [Switch('l', "A display label to print before any output.")]
        private string label = "Watch";

        [Variadic("The command to run.")]
        private List<string> command = new();

        private UserInterface _userInterface;
        private bool receivedPrompt;
        private CancellationToken _token;

        protected override async Task<CommandOutput> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            _userInterface = new UserInterface(this, this);
            _token = cancellationToken;

            if (!command.Any()) return new CommandOutput("Aborting: No command specified.");

            // Prevent overwriting the mirrored input line.
            if (overwrite) base.WriteLine(" ");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (receivedPrompt) return new CommandOutput();

                var request = new ShellRequest
                {
                    CommandResolver = new TokenCommandResolver(command.ToList()),
                    Session = context.Session,
                    Shell = context.Shell,
                    Windowed = false,
                    ExpandAliases = true,
                    NoPrompt = false
                };

                var result = await context.Shell.HandleCommandRequestAsync(request, cancellationToken, _userInterface);

                if (result.ErrorMessageValid)
                {
                    WriteLine(result.ErrorMessage);
                }

                if (result.Status is CommandResolutionStatus.Fail && killOnError) return new CommandOutput();
                await Task.Delay((int)(intervalSeconds * 1000), cancellationToken);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        public Task<object> HandlePrompt(Prompt prompt, CancellationToken cancellationToken)
        {
            receivedPrompt = true;
            WriteLine(MessageFormatter.Error("Watch command cannot handle prompts."));
            return null;
        }

        public CancellationToken GetCommandCancellationToken() => _token;

        public ShellSignalHandler GetSignalHandler()
        {
            void NullHandler(ShellSignal signal)
            {
                // no-op
            }
            return NullHandler;
        }

        public void Write(string message)
        {
            if (overwrite) OverWrite(message);
            else base.Write(label + ": " + message);
        }

        public void OverWrite(string message)
        {
            base.OverWrite(label + ": " + message);
        }

        public void WriteLine(string line)
        {
            if (overwrite) OverWrite(line);
            else base.WriteLine(label + ": " + line);
        }
    }
}
