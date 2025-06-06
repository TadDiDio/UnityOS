using System.Threading.Tasks;
using System.Collections.Generic;

namespace DeveloperConsole
{
    [Command("into", "Goes into the specified command and runs commands in its context.", true)]
    public class IntoCommand : ReplCommand
    {
        [Description("Command (path) to enter into.")]
        [VariadicArgs(true)] 
        private List<string> CommandPath; 
        
        private string _commandPath;
        
        protected override Task OnEnter(CommandContext context)
        {
            _commandPath = string.Join(".", CommandPath);
            return Task.CompletedTask;
        }

        public override string GetPromptLabel() => _commandPath;
        
        // TODO: There is probably a way smoother way of doing this than just testing likely cases
        protected override async Task<string> HandleInput(CommandContext context, string input)
        {
            ShellApplication.CommandRunResult result;
            if (input.ToLower().Trim().StartsWith("help"))
            {
                // Manual 'help' command override
                string rest = input.Trim()[4..] == null ? "" : input.Trim()[4..];
                result = await context.Shell.RunInput($"help {_commandPath} {rest}" );
            }
            else
            {
                // Execute as is
                result = await context.Shell.RunInput($"{_commandPath} {input}" );
            }
            
            // TODO: When cancelling is allowed make sure that we don't just run the rest of this stuff
            if (!result.Success)
            {
                // Try reversing input in the case of commands like help
                result = await context.Shell.RunInput($"{input} {_commandPath}");
            }

            if (!result.Success)
            {
                // Fallback on just single entered command in the case of commands like clear
                result = await context.Shell.RunInput(input);
            }
            return context.Shell.GetOutputStringFromResult(result);
        }
    }
}