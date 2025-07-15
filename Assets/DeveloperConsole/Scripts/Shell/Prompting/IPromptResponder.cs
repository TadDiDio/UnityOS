using System.Threading;
using System.Threading.Tasks;

namespace DeveloperConsole.Core.Shell
{
    public interface IPromptResponder
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
        public CancellationToken GetCommandCancellationToken();

        /// <summary>
        /// Gets the signal handler for this client.
        /// </summary>
        public ShellSignalHandler GetSignalHandler();
    }
}
