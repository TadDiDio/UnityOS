using System;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Core.Shell;
using DeveloperConsole.Parsing.TypeAdapting.Types;

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

        public override async Task<bool> Validate(CommandContext context, CancellationToken cancellationToken)
        {
            var prompt = Prompt.Confirmation(_message);
            var result = await context.Session.PromptAsync<ConfirmationResult>(context.CommandId, prompt, cancellationToken);
            return result.Success;
        }

        public override string OnValidationFailedMessage()
        {
            return "Operation cancelled";
        }
    }
}
