using DeveloperConsole.Core.Shell;

namespace DeveloperConsole.Command
{
    public interface ISignalContext { }

    public class SignalContext : ISignalContext
    {
        public SignalEmitter Emitter;

        public SignalContext(SignalEmitter emitter)
        {
            Emitter = emitter;
        }
    }
}
