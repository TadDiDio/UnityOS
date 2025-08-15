using System;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Core.Shell;
using DeveloperConsole.Scripts.Shell.Prompting;

namespace DeveloperConsole.Command
{
    public interface IPromptContext { }

    public class PromptContext : IPromptContext
    {
        private PromptManager _prompt;

        public PromptContext(PromptManager prompt) { _prompt = prompt; }

        public async Task<T> PromptAsync<T>(Prompt prompt, CancellationToken token) => await _prompt.PromptAsync<T>(prompt, token);

        public IDisposable PushPromptPrefixScope(string prefix) => _prompt.PushPromptPrefixScope(prefix);
    }
}
