using System.Threading.Tasks;

namespace DeveloperConsole.Core.Shell
{
    public interface IPromptResponder
    {
        /// <summary>
        /// Handles responding to a prompt.
        /// </summary>
        /// <param name="prompt">The requested prompt.</param>
        /// <returns>The result.</returns>
        public Task<object> HandlePrompt(Prompt prompt);

        /// <summary>
        /// Gets the signal handler for this client.
        /// </summary>
        public ShellSignalHandler GetSignalHandler();
    }
}
