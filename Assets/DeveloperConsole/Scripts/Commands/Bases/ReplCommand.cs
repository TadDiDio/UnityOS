using System.Reflection;
using System.Threading.Tasks;

namespace DeveloperConsole
{
    public abstract class ReplCommand : AsyncCommand
    {
        private bool _waitingForInput = true;
        private string _input;
        public override async Task<CommandResult> ExecuteAsync(CommandContext context)
        {
            await OnEnter(context);

            while (true)
            {
                while (_waitingForInput) await Task.Yield();
                _waitingForInput = true;
                
                context.Shell.SendOutput(_input, true);
                
                if (_input == "exit") break;
                var output = await HandleInput(context, _input);
                
                context.Shell.SendOutput(output, false);
            }
            
            return await OnReturn(context);
        }

        protected virtual Task OnEnter(CommandContext context) => Task.CompletedTask;
        protected virtual async Task<CommandResult> OnReturn(CommandContext context)
        {
            return await Task.FromResult(new CommandResult());
        }

        public void OnInput(string input)
        {
            _waitingForInput = false;
            _input = input;
        }

        protected abstract Task<string> HandleInput(CommandContext context, string input);

        public virtual string GetPromptLabel()
        {
            return GetType().GetCustomAttribute<CommandAttribute>()?.Name;
        }
    }
}