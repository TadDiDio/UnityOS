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

    public struct UserInterface
    {
        /// <summary>
        /// A prompt responder.
        /// </summary>
        public IPromptResponder Responder;

        /// <summary>
        /// An output channel.
        /// </summary>
        public IOutputChannel Output;

        /// <summary>
        /// A collection of I/O devices that represents a user interface.
        /// </summary>
        /// <param name="responder">A prompt responder.</param>
        /// <param name="output">An output channel.</param>
        public UserInterface(IPromptResponder responder, IOutputChannel output)
        {
            Responder = responder;
            Output = output;
        }
    }

    /// <summary>
    /// Owns the interaction and state context for a shell session.
    /// </summary>
    public class ShellSession
    {
        private IShellApplication _shell;

        private bool _promptingAllowed = true;
        private Dictionary<string, string> _aliasTable = new();

        private Guid _sessionId;
        private Dictionary<Guid, UserInterface> _interfaces = new();

        // TODO: get this file location from somewhere more suitable like a config.
        private const string StartupFilePath = "Assets/DeveloperConsole/Resources/console_start.txt";

        /// <summary>
        /// Creates a new shell session for a human interface.
        /// </summary>
        /// <param name="shell">The shell.</param>
        /// <param name="humanInterface">The human interface.</param>
        /// <param name="sessionId">The session id.</param>
        public ShellSession(IShellApplication shell, IHumanInterface humanInterface, Guid sessionId)
        {
            Initialize(sessionId, shell, humanInterface, new List<IOutputChannel> {humanInterface});
        }


        /// <summary>
        /// Creates a new shell session.
        /// </summary>
        /// <param name="shell">The shell.</param>
        /// <param name="responder">The prompt responder.</param>
        /// <param name="sessionId">The session id.</param>
        /// <param name="outputs">0 or more output channels.</param>
        public ShellSession(
            IShellApplication shell,
            IPromptResponder responder,
            Guid sessionId,
            List<IOutputChannel> outputs = null)
        {
            Initialize(sessionId, shell, responder, outputs);
        }

        private void Initialize(Guid sessionId, IShellApplication shell, IPromptResponder responder,
            List<IOutputChannel> outputs = null)
        {
            _shell = shell;
            _sessionId = sessionId;
            _interfaces[_sessionId] = new UserInterface(responder, new CompositeOutputChannel(outputs));

            var startBatch = FileBatcher.BatchFile(StartupFilePath);
            _ = CommandPromptLoop(startBatch);
        }

        private async Task CommandPromptLoop(CommandBatch startBatch)
        {
            try
            {
                await SubmitBatch(startBatch, _interfaces[_sessionId].Responder.GetCommandCancellationToken());
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
                    var token = _interfaces[_sessionId].Responder.GetCommandCancellationToken();
                    var batch = await PromptAsync<CommandBatch>(_sessionId, Prompt.Command(), token);
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
                    var output = await _shell.HandleCommandRequestAsync(shellRequest, token, GetDefaultInterface());
                    _promptingAllowed = true;

                    switch (output.Status)
                    {
                        case CommandResolutionStatus.Success:
                            previousStatus = Status.Success;
                            break;

                        case CommandResolutionStatus.AliasExpansion:
                            aliasExpanded = true;
                            var batcher = new DefaultCommandBatcher();
                            var insertBatch = batcher.GetBatchFromTokens(output.Tokens);
                            requests.InsertRange(i + 1, insertBatch.Requests);
                            break;

                        case CommandResolutionStatus.Fail:
                            previousStatus = Status.Fail;
                            if (output.ErrorMessageValid)
                            {
                                WriteLine(_sessionId, output.ErrorMessage);
                            }
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
        /// <param name="id">The id of the object invoking this.</param>
        /// <param name="prompt">The prompt to give.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the prompt.</param>
        /// <typeparam name="T">The response type.</typeparam>
        /// <returns>The response.</returns>
        /// <exception cref="InvalidOperationException">Throws if incorrect response type given.</exception>
        public async Task<T> PromptAsync<T>(Guid id, Prompt prompt, CancellationToken cancellationToken)
        {
            // BUG: Unnoticed so far but this likely causes problems for async commands running in parallel.
            if (!_promptingAllowed) throw new InvalidOperationException("Cannot prompt in a batch that does not allow it.");

            try
            {
                while (true)
                {
                    IPromptResponder promptResponder = _interfaces[id].Responder;
                    if (promptResponder == null) throw new InvalidOperationException($"Could not find a prompt responder for object {id}");

                    var response = await promptResponder.HandlePrompt(prompt, cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();

                    if (response is T typed)
                    {
                        if (prompt.Validator.Invoke(typed)) return typed;

                        WriteLine(_sessionId, MessageFormatter.Error($"Cannot convert {response.GetType().FullName} to type {typeof(T).FullName}."));
                        continue;
                    }

                    if (response is not string stringResponse)
                        throw new InvalidOperationException(
                            $"{promptResponder.GetType().Name} responded with {response.GetType().Name} " +
                            $"when asked for {prompt.RequestedType.Name}!");

                    if (stringResponse is "") continue;

                    if (!ConsoleAPI.Parsing.CanAdaptType(typeof(T)))
                    {
                        throw new InvalidOperationException($"No type adapter registered for type {typeof(T).Name}");
                    }

                    var result = ConsoleAPI.Parsing.AdaptTypeFromString(typeof(T), stringResponse);

                    if (!result.Success)
                    {
                        WriteLine(_sessionId, MessageFormatter.Error($"Cannot convert {response.GetType().FullName} to type {typeof(T).FullName}."));
                        continue;
                    }

                    if (prompt.Validator.Invoke(result.As<T>())) return result.As<T>();
                    WriteLine(_sessionId, MessageFormatter.Error($"Cannot convert {response.GetType().FullName} to type {typeof(T).FullName}."));

                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                WriteLine(_sessionId, MessageFormatter.Error(e.Message));
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

        /// <summary>
        /// Gets the default user interface for the session.
        /// </summary>
        /// <returns></returns>
        public UserInterface GetDefaultInterface() => _interfaces[_sessionId];

        /// <summary>
        /// Maps an id to an interface.
        /// </summary>
        /// <param name="id">The id to register for.</param>
        /// <param name="userInterface">The interface to map to.</param>
        public void RegisterInterfaceId(Guid id, UserInterface userInterface)
        {
            _interfaces[id] = userInterface;
        }

        /// <summary>
        /// Removes an id from the interface table.
        /// </summary>
        /// <param name="id">The id to remove.</param>
        public void UnregisterInterfaceId(Guid id)
        {
            _interfaces.Remove(id);
        }

        /// <summary>
        /// Writes a message to the output channel.
        /// </summary>
        /// <param name="id">The id of the writer.</param>
        /// <param name="message">The message.</param>
        public void Write(Guid id, object message)
        {
            if (!ValidateId(id))
            {
                Log.Error($"Id {id} not found when writing to output channel.");
                return;
            }

            _interfaces[id].Output.WriteLine(message.ToString());
        }

        /// <summary>
        /// Overwrites the current message in the output channel.
        /// </summary>
        /// <param name="id">The id of the writer.</param>
        /// <param name="message">The message.</param>
        public void OverWrite(Guid id, object message)
        {
            if (!ValidateId(id))
            {
                Log.Error($"Id {id} not found when writing to output channel.");
                return;
            }

            _interfaces[id].Output.OverWrite(message.ToString());
        }

        /// <summary>
        /// Writes a line to the output channel.
        /// </summary>
        /// <param name="id">The id of the writer.</param>
        /// <param name="line">The line.</param>
        public void WriteLine(Guid id, object line)
        {
            if (!ValidateId(id))
            {
                Log.Error($"Id {id} not found when writing to output channel.");
                return;
            }

            _interfaces[id].Output.WriteLine(line.ToString());
        }

        private bool ValidateId(Guid id) => _interfaces.ContainsKey(id);


        /// <summary>
        /// Signals the frontend.
        /// </summary>
        /// <param name="signal">The signal.</param>
        public void Signal(ShellSignal signal)
        {
            _interfaces[_sessionId].Responder.GetSignalHandler()?.Invoke(signal);
        }
    }
}
