using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command;
using DeveloperConsole.Core.Shell;
using DeveloperConsole.IO;

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

        private bool _receivedPrompt;
        private CancellationToken _token;
        private bool _kill;
        private UserInterface _userInterface;

        protected override async Task<CommandOutput> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            _token = cancellationToken;
            _userInterface = new UserInterface(this, this);

            if (!command.Any()) return new CommandOutput("Aborting: No command specified.");

            // Prevent overwriting the mirrored input line.
            if (overwrite) base.WriteLine(" ");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (_receivedPrompt) return new CommandOutput();

                var batch = new CommandBatch
                {
                    AllowPrompting = true
                };

                var request = new FrontEndCommandRequest
                {
                    Resolver = new TokenCommandResolver(command.ToList()),
                    Windowed = false,
                    Condition = CommandCondition.Always
                };

                batch.Requests.Add(request);

                await context.Session.SubmitBatch(batch, _userInterface, OnCommandResult);

                if (_kill) break;

                await Task.Delay((int)(intervalSeconds * 1000), cancellationToken);
            }

            return new CommandOutput();
        }

        private void OnCommandResult(CommandExecutionResult result)
        {
            if (killOnError && result.Status is CommandResolutionStatus.Fail)
            {
                _kill = true;
            }
        }

        public Task<object> HandlePrompt(Prompt prompt, CancellationToken cancellationToken)
        {
            _receivedPrompt = true;
            WriteLine(MessageFormatter.Error("Watch command cannot handle prompts."));
            return null;
        }

        public CancellationToken GetCommandCancellationToken()
        {
            return _token;
        }

        public ShellSignalHandler GetSignalHandler()
        {
            void NullHandler(ShellSignal signal) { /* no-op */}
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
