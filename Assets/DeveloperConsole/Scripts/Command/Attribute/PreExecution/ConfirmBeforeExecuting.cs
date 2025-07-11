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
            var prompt = new PromptContext
            {
                Message = "Are you sure you want to continue? (y/n)",
                Options = new object[] { "y", "n" }
            };
            
            var response = await context.Session.Prompt<TextInput>(prompt);
            
            
            return response.Input.Trim().ToLower() == "y";
        }

        
        public override string OnValidationFailedMessage() => "Operation cancelled";
    }
}