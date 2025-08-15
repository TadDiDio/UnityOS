using System.Threading;
using System.Threading.Tasks;

namespace DeveloperConsole.Core.Shell
{
    public interface IPromptable
    {
        /// <summary>
        /// Handles responding to a prompt.
        /// </summary>
        /// <param name="prompt">The requested prompt.</param>
        /// <param name="cancellationToken">A cancellation token to exit execution.</param>
        /// <returns>The result.</returns>
        public Task<object> HandlePrompt(Prompt prompt, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a cancellationToken to cancel executing commands.
        /// </summary>
        /// <returns>The cancellation token.</returns>
        public CancellationToken GetPromptCancellationToken();

        /// <summary>
        /// Gets the signal handler for this client.
        /// </summary>
        public ShellSignalHandler GetSignalHandler();

        /// <summary>
        /// Sets the prompt header to display. Should be used as is without further processing.
        /// </summary>
        /// <param name="header">The header.</param>
        public void SetPromptHeader(string header);
    }
}
