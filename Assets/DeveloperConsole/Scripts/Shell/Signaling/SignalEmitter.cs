namespace DeveloperConsole.Core.Shell
{
    public class SignalEmitter
    {
        private IPromptable _promptable;

        public SignalEmitter(IPromptable promptable)
        {
            _promptable = promptable;
        }

        public void Signal(ShellSignal signal)
        {
            _promptable.GetSignalHandler().Invoke(signal);
        }
    }
}
