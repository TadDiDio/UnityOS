using System.Threading.Tasks;

namespace DeveloperConsole
{
    public class ConfirmBeforeExecuting : PreExecutionValidatorAttribute
    {
        public override async Task<bool> Validate(CommandContext context)
        {
            context.Shell.SendOutput("Are you sure you want to continue? (y/n)", false);
            var response = await context.Shell.WaitForInput();

            return response.Trim().ToLower() == "y";
        }

        public override string OnValidationFailedMessage() => "Operation cancelled";
    }
}