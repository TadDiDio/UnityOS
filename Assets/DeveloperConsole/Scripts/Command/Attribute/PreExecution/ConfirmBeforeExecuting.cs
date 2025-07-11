using System.Threading.Tasks;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Requires a command to be confirmed by the user before running.
    /// </summary>
    public class ConfirmBeforeExecuting : PreExecutionValidatorAttribute
    {
        public override async Task<bool> Validate(CommandContext context)
        {
            return await context.Session.Confirm("Are you sure you want to continue?");
        }

        public override string OnValidationFailedMessage() => "Operation cancelled";
    }
}
