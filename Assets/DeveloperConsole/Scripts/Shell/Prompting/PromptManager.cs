using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command.Execution;
using DeveloperConsole.Core.Shell;
using DeveloperConsole.IO;
using DeveloperConsole.Parsing.Graph;

namespace DeveloperConsole.Shell.Prompting
{
    public class PromptManager
    {
        public IPromptable Promptable;
        private IOutputChannel _output;

        private ShellSession _session;

        private string _promptEnd;
        private Stack<string> _promptStack = new();

        private bool _promptActive;
        private readonly object _lock = new();

        private bool _autoRetryPrompt;

        public PromptManager(IPromptable promptable, IOutputChannel output, ShellSession session, bool autoRetryPrompt = true)
        {
            Promptable = promptable;
            _output = output;
            _session = session;
            _autoRetryPrompt = autoRetryPrompt;
        }

        /// <summary>
        /// Call this method in a using tag to push a prompt header for the duration.
        /// </summary>
        /// <param name="prefix">The prefix to add.</param>
        public IDisposable PushPromptPrefixScope(string prefix)
        {
            PushPromptPrefix(prefix);
            return new DisposableAction(PopPromptPrefix);
        }

        /// <summary>
        /// Gets this prompts cancellation token.
        /// </summary>
        /// <returns>The token.</returns>
        public CancellationToken GetPromptCancellationToken()
        {
            return Promptable.GetPromptCancellationToken();
        }

        /// <summary>
        /// Initializes the prompt.
        /// </summary>
        /// <param name="header"></param>
        public void InitializePromptHeader(string header)
        {
            _promptEnd = header;
            Promptable.SetPromptHeader(header);
        }

        /// <summary>
        /// Handles exclusive prompting. Only a single entity may prompt at a time.
        /// </summary>
        /// <param name="prompt">The submitted prompt.</param>
        /// <param name="token">A cancellation token.</param>
        /// <typeparam name="T">The type of response to ask for.</typeparam>
        /// <returns>The response or null if no retry was attempted.</returns>
        /// <exception cref="InvalidOperationException">Throws if another prompt is in session.</exception>
        public async Task<T> PromptAsync<T>(Prompt<T> prompt, CancellationToken token)
        {
            lock (_lock)
            {
                if (_promptActive) throw new InvalidOperationException($"Prompt was already active when a {prompt.Kind} prompt was received.");
                _promptActive = true;
            }

            try
            {
                bool condition = true;
                while (condition)
                {
                    token.ThrowIfCancellationRequested();

                    condition = _autoRetryPrompt;

                    var response = await Promptable.HandlePrompt(prompt, token);


                    // ====================================
                    // Handle correctly typed responses directly
                    // ====================================

                    if (response is T typed)
                    {
                        if (prompt.Validator.Invoke(typed)) return typed;

                        _output.WriteLine(Formatter.Error($"Cannot convert {response.GetType().FullName} to type {typeof(T).FullName}."));
                        continue;
                    }


                    // ====================================
                    // Fail loudly when the response is
                    // not a string or the correct type
                    // ====================================

                    if (response is not string stringResponse) throw new InvalidOperationException
                        ($"{Promptable.GetType().Name} responded with {response.GetType().Name} when asked for {{prompt.RequestedType.Name}}!");

                    if (stringResponse is "") continue;


                    // ====================================
                    // Fail loudly when the string response
                    // cannot be adapted by any registered
                    // adapter
                    // ====================================

                    if (!ConsoleAPI.Parsing.CanAdaptType(typeof(T)))
                    {
                        throw new InvalidOperationException($"No type adapter registered for type {typeof(T).Name}");
                    }

                    // Try adapting and show result
                    var result = ConsoleAPI.Parsing.AdaptTypeFromString(typeof(T), stringResponse);

                    if (!result.Success)
                    {
                        _output.WriteLine(Formatter.Error($"{result.ErrorMessage}"));
                        continue;
                    }

                    // Success
                    if (prompt.Validator.Invoke(result.As<T>())) return result.As<T>();

                    // Failure
                    _output.WriteLine(Formatter.Error($"Could not validate converting {response.GetType().FullName} to type {typeof(T).FullName}."));
                }
            }
            catch (OperationCanceledException)
            {
                // Caught by either Shell or ShellSession
                throw;
            }
            catch (Exception e)
            {
                // Display all other exceptions
                _output.WriteLine(Formatter.Error(e.Message));
                throw;
            }
            finally { lock (_lock) { _promptActive = false; } }

            throw new InvalidOperationException("Auto retry on invalid prompt is off and an invalid prompt was submitted.");
        }

        public async Task<CommandGraph> PromptAsync(Prompt<CommandGraph> prompt, CancellationToken token)
        {
            lock (_lock)
            {
                if (_promptActive)
                    throw new InvalidOperationException(
                        $"Prompt was already active when a {prompt.Kind} prompt was received.");
                _promptActive = true;
            }

            try
            {
                bool condition = true;
                while (condition)
                {
                    token.ThrowIfCancellationRequested();

                    condition = _autoRetryPrompt;
                    var response = await Promptable.HandlePrompt(prompt, token);

                    if (response is CommandGraph typed)
                    {
                        if (prompt.Validator.Invoke(typed)) return typed;

                        _output.WriteLine(Formatter.Error(
                            $"Cannot convert {response.GetType().FullName} to type {typeof(CommandGraph).FullName}."));
                        continue;
                    }

                    if (response is not string stringResponse) throw new InvalidOperationException
                        ($"{Promptable.GetType().Name} responded with {response.GetType().Name} when asked for {{prompt.RequestedType.Name}}!");

                    if (stringResponse is "") continue;

                    var result = new GraphParser().ParseToGraph(stringResponse, _session);

                    if (result.Succeeded) return result.Graph;

                    _output.WriteLine(Formatter.Error(result.ErrorMessage));
                }
            }
            catch (OperationCanceledException)
            {
                // Caught by either Shell or ShellSession
                throw;
            }
            catch (Exception e)
            {
                // Display all other exceptions
                _output.WriteLine(Formatter.Error(e.Message));
                throw;
            }
            finally { lock (_lock) { _promptActive = false; } }

            throw new InvalidOperationException("Auto retry on invalid prompt is off and an invalid prompt was submitted.");
        }

        private void PushPromptPrefix(string prefix)
        {
            _promptStack.Push(prefix);
            Promptable.SetPromptHeader(GetPromptHeader());
        }

        private void PopPromptPrefix()
        {
            _promptStack.TryPop(out _);
            Promptable.SetPromptHeader(GetPromptHeader());
        }

        private string GetPromptHeader() => string.Join("/", _promptStack.Reverse()) + _promptEnd;
    }
}
