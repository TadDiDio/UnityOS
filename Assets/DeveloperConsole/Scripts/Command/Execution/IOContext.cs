using DeveloperConsole.Core.Shell.Prompting;
using DeveloperConsole.IO;
using DeveloperConsole.Scripts.Shell.Prompting;


namespace DeveloperConsole.Core.Shell
{
    public sealed class IOContext
    {
        public IOutputChannel Output { get; }
        public PromptManager Prompt { get; }
        // TODO: Clean up this because if they give a signal emitter they must be a shell client which restricts rerouting io maybe
        public SignalEmitter SignalEmitter { get; }
        public IOContext(IPromptable prompt, IOutputChannel output, SignalEmitter emitter, IPromptWrapper promptWrapper = null)
        {
            Prompt = new PromptManager(prompt, output, promptWrapper ?? new RetryUntilSuccessPromptWrapper());
            Output = output;
            SignalEmitter = emitter;
        }
    }
}
