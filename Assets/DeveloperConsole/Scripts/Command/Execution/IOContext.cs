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

        private string _name;

        public IOContext(IPromptable prompt, IOutputChannel output, SignalEmitter emitter, ShellSession session, bool autoRetry = true, string name = null)
        {
            Prompt = new PromptManager(prompt, output, session, autoRetry);
            Output = output;
            SignalEmitter = emitter;
            ShellSession = session;
            _name = name;
        }

        public static IOContext CreateFromClient(IShellClient client, ShellSession session)
        {
            return new IOContext(client, client, new SignalEmitter(client), session);
        }

        public IOContext Clone(string name = null)
        {
            return new IOContext(
                prompt: Prompt.Promptable,
                output: Output,
                emitter: SignalEmitter,
                session: ShellSession,
                autoRetry: true,
                name: name ?? _name
            );
        }

        public override string ToString() => _name ?? "IOContext";
    }
}
