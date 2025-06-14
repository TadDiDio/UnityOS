using DeveloperConsole.IO;
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
            context.Output.Emit(new SimpleOutputMessage(context.Session, "Are you sure you want to continue? (y/n)"));
            
            var response = await context.Session.WaitForInput();

            return response.Input.Trim().ToLower() == "y";
        }

        
        public override string OnValidationFailedMessage() => "Operation cancelled";
    }
}