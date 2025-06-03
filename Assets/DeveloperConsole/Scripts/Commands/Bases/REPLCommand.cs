using System.Threading.Tasks;

namespace DeveloperConsole
{
    public abstract class REPLCommand : AsyncCommand
    {
        public override async Task<CommandResult> ExecuteAsync(CommandContext context)
        {
            await OnEnter(context);

            while (true)
            {
                // Read
                var input = await context.Shell.WaitForInput();
                if (input == "exit") break;
                
                // Evaluate/Print
                context.Shell.SendOutput(await OnEvaluate(context, input));
            }
            
            return await OnReturn(context);
        }

        protected virtual Task OnEnter(CommandContext context) => Task.CompletedTask;
        protected abstract Task<string> OnEvaluate(CommandContext context, string input);
        protected virtual async Task<CommandResult> OnReturn(CommandContext context)
        {
            return await Task.FromResult(new CommandResult());
        }

        public abstract string GetUserPrompt();
    }
}