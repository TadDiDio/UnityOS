using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.IO;

namespace DeveloperConsole.Core.Shell.Prompting
{
    public interface IPromptWrapper
    {
        /// <summary>
        /// Wraps a raw prompt request with additional logic.
        /// </summary>
        /// <param name="promptable">The prompt handler.</param>
        /// <param name="output">An output channel to display errors.</param>
        /// <param name="prompt">The prompt requested.</param>
        /// <param name="cancellationToken">The cancellation token to obey.</param>
        /// <return>Returns the result of the prompting.</return>
        public Task<PromptResult<T>> HandlePrompt<T>
        (
            IPromptable promptable,
            IOutputChannel output,
            Prompt prompt,
            CancellationToken cancellationToken
        );
    }
}
