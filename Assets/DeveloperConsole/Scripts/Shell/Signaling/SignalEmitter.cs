namespace DeveloperConsole.Core.Shell
{
    public class SignalEmitter
    {
        private IShellClient _client;

        public SignalEmitter(IShellClient client)
        {
            _client = client;
        }

        public void Signal(ShellSignal signal)
        {
            _client.GetSignalHandler().Invoke(signal);
        }
    }
}
