using System;
using System.Threading.Tasks;

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
        public GraphProcessor GraphProcessor { get; }

        public const string PromptEnd = "> ";

        private IShell _shell;
        private IOContext _defaultIOContext;

        // TODO: get this file location from somewhere more suitable like a config.
        private const string StartupFilePath = "Assets/DeveloperConsole/Resources/console_start.txt";

        /// <summary>
        /// Creates a new shell session for a human interface.
        /// </summary>
        /// <param name="shell">The shell.</param>
        /// <param name="client">The default client for this session.</param>
        public ShellSession(IShell shell, IShellClient client)
        {
            if (client == null)
            {
                Log.Error("The session was started with no client (null) which is not allowed.");
                return;
            }

            _shell = shell;
            _defaultIOContext = IOContext.CreateFromClient(client, this);
            _defaultIOContext.Prompt.InitializePromptHeader(PromptEnd);

            // TODO: Need to run this start up file
            // var startBatch = FileBatcher.BatchFile(StartupFilePath);

            GraphProcessor = new GraphProcessor(_shell, this, _defaultIOContext);

            _ = CommandPromptLoop();
        }

        private async Task CommandPromptLoop()
        {
            while (true) await PromptAndSubmit();
        }

        private async Task PromptAndSubmit()
        {
            try
            {
                var token = _defaultIOContext.Prompt.GetPromptCancellationToken();
                var graph = await _defaultIOContext.Prompt.PromptAsync(PromptFactory.Command(), token);
                await GraphProcessor.ProcessCommandGraphAsync(graph, token, _defaultIOContext);
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
            catch (Exception e)
            {
                Log.Error($"Exception thrown while prompting for command: {e}");
            }
        }
    }
}
