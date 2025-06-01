using System.Threading.Tasks;

namespace DeveloperConsole
{
    public class ComfirmBeforeExecuting : PreExecutionValidatorAttribute
    {
        public override async Task<bool> Validate(CommandContext context)
        {
            var response = await context.Shell.WaitForInput("Are you sure you want to continue? (Y/n)");

            return response.Trim().ToLower() == "y";
        }

        public override string OnValidationFailedMessage() => "Operation cancelled";
    }
}