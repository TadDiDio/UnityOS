using System;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Core.Shell;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Requires a command to be confirmed by the user before running.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfirmBeforeExecuting : PreExecutionValidatorAttribute
    {
        private string _message;
        public ConfirmBeforeExecuting(string message)
        {
            _message = message;
        }

        public override async Task<bool> Validate(FullCommandContext context, CancellationToken cancellationToken)
        {
            var prompt = PromptFactory.Confirmation(_message);
            var result = await context.Prompting.PromptAsync(prompt, cancellationToken);
            return result.Success;
        }

        public override string OnValidationFailedMessage() => "Operation cancelled";
    }
}
