using DeveloperConsole.Core.Shell;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// The context that the command executes in.
    /// </summary>
    public class CommandContext
    {
        /// <summary>
        /// The session this command is running in.
        /// </summary>
        public ShellSession Session;

        /// <summary>
        /// A shell for running commands.
        /// </summary>
        public ShellApplication Shell;

        /// <summary>
        /// The current environment.
        /// </summary>
        public UnityEnvironment Environment;
    }


    /// <summary>
    /// The different environments in Unity.
    /// </summary>
    public enum UnityEnvironment
    {
        EditMode,
        PlayMode,
        BuildMode
    }
}
