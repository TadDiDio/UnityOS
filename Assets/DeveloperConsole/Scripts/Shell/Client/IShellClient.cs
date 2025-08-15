using DeveloperConsole.IO;

namespace DeveloperConsole.Core.Shell
{
    public interface IShellClient : IPromptable, IOutputChannel
    {
        /// <summary>
        /// Gets the IO context for this client.
        /// </summary>
        /// <returns>The context.</returns>
        public IOContext GetIOContext();
    }
}
