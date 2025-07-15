using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Core.Shell;
using DeveloperConsole.Parsing.TypeAdapting.Types;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Requires a command to be confirmed by the user before running.
    /// </summary>
    public class ConfirmBeforeExecuting : PreExecutionValidatorAttribute
    {
        public override async Task<bool> Validate(CommandContext context, CancellationToken cancellationToken)
        {
            var prompt = Prompt.Confirmation("Are you sure you want to proceed?");
            var result = await context.Session.PromptAsync<ConfirmationResult>(prompt, cancellationToken);
            return result.Success;
        }

        public override string OnValidationFailedMessage() => "Operation canceled";
    }
}
