using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command;
using DeveloperConsole.IO;
using DeveloperConsole.Parsing;
using DeveloperConsole.Parsing.Tokenizing;

namespace DeveloperConsole.Core.Shell
{
    /// <summary>
    /// A delegate representing a shell signal handler.
    /// </summary>
    public delegate void ShellSignalHandler(ShellSignal signal);

    /// <summary>
    /// Owns the interaction and state context for a shell session.
    /// </summary>
    public class ShellSession : IDisposable
    {
        private IPromptResponder _promptResponder;
        private IShellApplication _shell;
        private CompositeOutputChannel _output;

        private bool _promptingAllowed = true;
        private string _promptHeader = "";
        private ShellSignalHandler _onShellSignal;
        private Dictionary<string, string> _aliasTable = new();

        // TODO: get this file location from somewhere more suitable like a config.
        private const string StartupFilePath = "Assets/DeveloperConsole/Resources/console_start.txt";

        /// <summary>
        /// Creates a new shell session for a human interface.
        /// </summary>
        /// <param name="shell">The shell.</param>
        /// <param name="humanInterface">The human interface.</param>
        public ShellSession(IShellApplication shell, IHumanInterface humanInterface)
        {
            Initialize(shell, humanInterface, new List<IOutputChannel> {humanInterface});
        }


        /// <summary>
        /// Creates a new shell session.
        /// </summary>
        /// <param name="shell">The shell.</param>
        /// <param name="responder">The prompt responder.</param>
        /// <param name="outputs">0 or more output channels.</param>
        public ShellSession(
            IShellApplication shell,
            IPromptResponder responder,
            List<IOutputChannel> outputs = null)
        {
            Initialize(shell, responder, outputs);
        }

        private void Initialize(IShellApplication shell, IPromptResponder responder,
            List<IOutputChannel> outputs = null)
        {
            _shell = shell;
            _promptResponder = responder;
            _onShellSignal += responder.GetSignalHandler();
            _output = new CompositeOutputChannel(outputs);

            var startBatch = FileBatcher.BatchFile(StartupFilePath);
            _ = CommandPromptLoop(startBatch);
        }


        private async Task CommandPromptLoop(CommandBatch startBatch)
        {
            try
            {
                await SubmitBatch(startBatch, _promptResponder.GetCommandCancellationToken());
            }
            catch (Exception e)
            {
                // This should not happen. It should be caught earlier
                Log.Exception(e);
            }

            while (true)
            {
                try
                {
                    var token = _promptResponder.GetCommandCancellationToken();
                    var batch = await PromptAsync<CommandBatch>(Prompt.Command(), token);
                    await SubmitBatch(batch, token);
                }
                catch (OperationCanceledException)
                {
                    // Ignored: This is expected
                }
                catch (Exception e)
                {
                    // This should not happen. It should be caught earlier
                    Log.Exception(e);
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }


        private async Task SubmitBatch(CommandBatch batch, CancellationToken token)
        {
            try
            {
                var aliasExpanded = false;
                var previousStatus = Status.Success;

                var requests = new List<FrontEndCommandRequest>(batch.Requests);

                for (int i = 0; i < requests.Count; i++)
                {
                    var request = requests[i];
                    if (token.IsCancellationRequested) return;
                    if (!request.Condition.AllowsStatus(previousStatus)) continue;

                    var shellRequest = new ShellRequest
                    {
                        Session = this,
                        ExpandAliases = !aliasExpanded,
                        Windowed = request.Windowed,
                        CommandResolver = request.Resolver
                    };

                    aliasExpanded = false;

                    _promptingAllowed = batch.AllowPrompting && !request.Windowed;
                    var output = await _shell.HandleCommandRequestAsync(shellRequest, token);
                    _promptingAllowed = true;

                    switch (output.Status)
                    {
                        case CommandResolutionStatus.Success:
                            previousStatus = Status.Success;
                            WriteLine(output.CommandOutput.Message);
                            break;

                        case CommandResolutionStatus.AliasExpansion:
                            aliasExpanded = true;
                            var batcher = new DefaultCommandBatcher();
                            var insertBatch = batcher.GetBatchFromTokens(output.Tokens);
                            requests.InsertRange(i + 1, insertBatch.Requests);
                            break;

                        case CommandResolutionStatus.Fail:
                            previousStatus = Status.Fail;
                            WriteLine(output.ErrorMessage);
                            break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Ignored: This case happens when a user cancels a command request prompt which is not an issue
            }
        }


        /// <summary>
        /// Prompts the frontend.
        /// </summary>
        /// <param name="prompt">The prompt to give.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the prompt.</param>
        /// <typeparam name="T">The response type.</typeparam>
        /// <returns>The response.</returns>
        /// <exception cref="InvalidOperationException">Throws if incorrect response type given.</exception>
        public async Task<T> PromptAsync<T>(Prompt prompt, CancellationToken cancellationToken)
        {
            // BUG: Unnoticed so far but this likely causes problems for async commands running in parallel.
            if (!_promptingAllowed) throw new InvalidOperationException("Cannot prompt in a batch that does not allow it.");

            try
            {
                while (true)
                {
                    var response = await _promptResponder.HandlePrompt(prompt, cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();

                    if (response is T typed)
                    {
                        if (prompt.Validator.Invoke(typed)) return typed;

                        WriteLine(MessageFormatter.Error($"Cannot convert {response.GetType().FullName} to type {typeof(T).FullName}."));
                        continue;
                    }

                    if (response is not string stringResponse)
                        throw new InvalidOperationException(
                            $"{_promptResponder.GetType().Name} responded with {response.GetType().Name} " +
                            $"when asked for {prompt.RequestedType.Name}!");

                    if (stringResponse is "") continue;

                    if (!ConsoleAPI.Parsing.CanAdaptType(typeof(T)))
                    {
                        throw new InvalidOperationException($"No type adapter registered for type {typeof(T).Name}");
                    }

                    var result = ConsoleAPI.Parsing.AdaptTypeFromString(typeof(T), stringResponse);

                    if (!result.Success)
                    {
                        WriteLine(MessageFormatter.Error($"Cannot convert {response.GetType().FullName} to type {typeof(T).FullName}."));
                        continue;
                    }

                    if (prompt.Validator.Invoke(result.As<T>())) return result.As<T>();
                    WriteLine(MessageFormatter.Error($"Cannot convert {response.GetType().FullName} to type {typeof(T).FullName}."));

                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                WriteLine(MessageFormatter.Error(e.Message));
                throw;
            }
        }


        /// <summary>
        /// Adds an alias.
        /// </summary>
        /// <param name="key">The alias to add.</param>
        /// <param name="value">The value of the alias.</param>
        public void AddAlias(string key, string value)
        {
            _aliasTable[key] = value;
        }


        /// <summary>
        /// Removes an aliase.
        /// </summary>
        /// <param name="key">The alias to remove.</param>
        public void RemoveAlias(string key)
        {
            _aliasTable.Remove(key);
        }


        /// <summary>
        /// Gets all aliases.
        /// </summary>
        /// <returns>The alias table.</returns>
        public Dictionary<string, string> GetAliases() => _aliasTable;


        /// <summary>
        /// Replaces a token with its alias.
        /// </summary>
        /// <param name="token">The token to replace.</param>
        /// <param name="replaced">Tells if an alias was applied.</param>
        /// <returns>The tokenized alias or the token if it doesn't have one.</returns>
        public List<string> GetAlias(string token, out bool replaced)
        {
            if (_aliasTable.TryGetValue(token, out var value))
            {
                var result = ConsoleAPI.Parsing.Tokenize(value);

                replaced = result.Status is not TokenizationStatus.Empty;
                return replaced ? result.Tokens : new List<string> { token };
            }

            replaced = false;
            return new List<string> { token };
        }

        public void AppendPromptHeader(string header) => _promptHeader += header;
        public void RemoveFromPromptHeader(string header) => _promptHeader = _promptHeader.Replace(header, "");


        /// <summary>
        /// Writes to the output channel.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Write(object message)
        {
            _output.Write(message.ToString());
        }


        /// <summary>
        /// Writes a line to the output channel.
        /// </summary>
        /// <param name="line">The line.</param>
        public void WriteLine(object line)
        {
            _output.WriteLine(line.ToString());
        }


        /// <summary>
        /// Signals the frontend.
        /// </summary>
        /// <param name="signal">The signal.</param>
        public void Signal(ShellSignal signal)
        {
            _onShellSignal?.Invoke(signal);
        }

        public void Dispose()
        {
            _onShellSignal = null;
        }
    }
}
