namespace DeveloperConsole.Core.Shell
{
    /// <summary>
    /// An abstract signal to message arbitrary data to clients.
    /// </summary>
    public abstract class ShellSignal { }

    /// <summary>
    /// Signal indicating to clear the screen.
    /// </summary>
    public class ClearSignal : ShellSignal { }
}
