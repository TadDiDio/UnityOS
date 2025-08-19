using DeveloperConsole.IO;
using DeveloperConsole.Shell.Prompting;

namespace DeveloperConsole.Core.Shell
{
    public sealed class IOContext
    {
        public IOutputChannel Output { get; }
        public PromptManager Prompt { get; }
        public SignalEmitter SignalEmitter { get; }
        public ShellSession ShellSession { get; }

        public IOContext(IPromptable prompt, IOutputChannel output, SignalEmitter emitter, ShellSession session)
        {
            Prompt = new PromptManager(prompt, output, session);
            Output = output;
            SignalEmitter = emitter;
            ShellSession = session;
        }

        public static IOContext CreateFromClient(IShellClient client, ShellSession session)
        {
            return new IOContext(client, client, new SignalEmitter(client), session);
        }
    }
}
