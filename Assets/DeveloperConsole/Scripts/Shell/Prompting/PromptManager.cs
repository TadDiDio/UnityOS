using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Core.Shell;
using DeveloperConsole.Core.Shell.Prompting;
using DeveloperConsole.IO;
using DeveloperConsole.Scripts.Utils;

namespace DeveloperConsole.Scripts.Shell.Prompting
{
    public class PromptManager
    {
        private IPromptable _prompt;
        private IPromptWrapper _wrapper;
        private IOutputChannel _output;

        private string _promptEnd;
        private Stack<string> _promptStack = new();

        private bool _promptActive;
        private readonly object _lock = new();

        public PromptManager(IPromptable prompt, IOutputChannel output, IPromptWrapper wrapper)
        {
            _prompt = prompt;
            _wrapper = wrapper;
            _output = output;
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
            return _prompt.GetPromptCancellationToken();
        }

        /// <summary>
        /// Initializes the prompt.
        /// </summary>
        /// <param name="header"></param>
        public void InitializePromptHeader(string header)
        {
            _promptEnd = header;
            _prompt.SetPromptHeader(header);
        }

        /// <summary>
        /// Handles exclusive prompting. Only a single entity may prompt at a time.
        /// </summary>
        /// <param name="prompt">The submitted prompt.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <typeparam name="T">The type of response to ask for.</typeparam>
        /// <returns>The response.</returns>
        /// <exception cref="InvalidOperationException">Throws if another prompt is in session.</exception>
        public async Task<T> PromptAsync<T>(Prompt prompt, CancellationToken cancellationToken)
        {
            lock (_lock)
            {
                if (_promptActive) throw new InvalidOperationException
                    ($"Prompt was already active when a {{prompt.Kind}} prompt was received.");

                _promptActive = true;
            }

            try
            {
                var result = await _wrapper.HandlePrompt<T>(_prompt, _output, prompt, cancellationToken);

                if (!result.Success)
                {
                    _output.WriteLine(result.ErrorMessage);
                }

                return result.Value;
            }
            finally
            {
                lock (_lock)
                {
                    _promptActive = false;
                }
            }
        }

        private void PushPromptPrefix(string prefix)
        {
            _promptStack.Push(prefix);
            _prompt.SetPromptHeader(GetPromptHeader());
        }

        private void PopPromptPrefix()
        {
            _promptStack.TryPop(out _);
            _prompt.SetPromptHeader(GetPromptHeader());
        }

        private string GetPromptHeader() => string.Join("/", _promptStack.Reverse()) + _promptEnd;
    }
}
