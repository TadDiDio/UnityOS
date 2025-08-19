using System;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command.Execution;
using DeveloperConsole.Core.Shell;
using DeveloperConsole.Shell.Prompting;

namespace DeveloperConsole.Command
{
    public interface IPromptContext { }

    public class PromptContext : IPromptContext
    {
        private PromptManager _prompt;

        public PromptContext(PromptManager prompt)
        {
            _prompt = prompt;
        }

        public async Task<T> PromptAsync<T>(Prompt<T> prompt, CancellationToken token) =>
            await _prompt.PromptAsync(prompt, token);

        public async Task<CommandGraph> PromptAsync(Prompt<CommandGraph> prompt, CancellationToken token) =>
            await _prompt.PromptAsync(prompt, token);

        public IDisposable PushPromptPrefixScope(string prefix) => _prompt.PushPromptPrefixScope(prefix);
    }
}
