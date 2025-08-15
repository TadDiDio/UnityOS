namespace DeveloperConsole.Core.Shell
{
    /// <summary>
    /// A delegate representing a shell signal handler.
    /// </summary>
    public delegate void ShellSignalHandler(ShellSignal signal);

    /// <summary>
    /// An abstract signal to message arbitrary data to clients.
    /// </summary>
    public abstract class ShellSignal { }

    /// <summary>
    /// Signal indicating to clear the screen.
    /// </summary>
    public class ClearSignal : ShellSignal { }
}
