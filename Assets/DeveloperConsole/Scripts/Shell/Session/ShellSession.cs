using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command;

namespace DeveloperConsole.Core.Shell
{
    /// <summary>
    /// Owns the interaction and state context for a shell session.
    /// </summary>
    public class ShellSession
    {
        /// <summary>
        /// A manager for handling aliases.
        /// </summary>
        public AliasManager AliasManager { get; } = new();

        /// <summary>
        /// A module for submitting commands.
        /// </summary>
        public CommandSubmitter CommandSubmitter { get; }

        public const string PromptEnd = "> ";

        private IShellApplication _shell;
        private IOContext _defaultIOContext;

        // TODO: get this file location from somewhere more suitable like a config.
        private const string StartupFilePath = "Assets/DeveloperConsole/Resources/console_start.txt";

        /// <summary>
        /// Creates a new shell session for a human interface.
        /// </summary>
        /// <param name="shell">The shell.</param>
        /// <param name="defaultIOContext">The default IOContext to route to.</param>
        public ShellSession(IShellApplication shell, IOContext defaultIOContext)
        {
            if (defaultIOContext == null)
            {
                Log.Error("The session was started with no IOContext (null) which is not allowed.");
                return;
            }

            _shell = shell;
            _defaultIOContext = defaultIOContext;
            _defaultIOContext.Prompt.InitializePromptHeader(PromptEnd);

            // TODO: Start file not running right now
            // var startBatch = FileBatcher.BatchFile(StartupFilePath);

            CommandSubmitter = new CommandSubmitter(_shell, this, _defaultIOContext);

            _ = CommandPromptLoop();
        }

        private async Task CommandPromptLoop()
        {
            while (true)
            {
                await PromptAndSubmit();
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private async Task PromptAndSubmit()
        {
            try
            {
                var token = _defaultIOContext.Prompt.GetPromptCancellationToken();
                var batch = await _defaultIOContext.Prompt.PromptAsync<CommandBatch>(Prompt.Command(), token);
                await CommandSubmitter.SubmitBatch(batch, token, _defaultIOContext);
            }
            catch (OperationCanceledException)
            {
                // Ignored: This happens when the user cancels with nothing running.
            }
            catch (Exception e)
            {
                Log.Error($"An unhandled exception occured. This should not happen: {e}");
            }
        }
    }
}
