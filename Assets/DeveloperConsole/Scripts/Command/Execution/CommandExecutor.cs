using System;
using System.Reflection;
using System.Threading.Tasks;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Executes commands.
    /// </summary>
    public class CommandExecutor : ICommandExecutor
    {
        public async Task<CommandExecutionResult> ExecuteCommand(CommandExecutionRequest executionRequest)
        {
            // 1. Resolve
            CommandRequest request = executionRequest.Request;
            var resolveResult = request.CommandResolver.Resolve(request.ShellSession);
            if (resolveResult.Status is not Status.Success)
            {
                return CommandExecutionResult.Fail(resolveResult.ErrorMessage);
            }
            
            // 2. Build context
            // TODO: Fill this out
            var context = new CommandContext();
            
            // 3. Pre-execution validation
            ICommand command = resolveResult.Command;
            var validators = command.GetType().GetCustomAttributes<PreExecutionValidatorAttribute>();

            foreach (var validator in validators)
            {
                if (await validator.Validate(context)) continue;
                return CommandExecutionResult.Fail(validator.OnValidationFailedMessage());
            }
            
            // 4. Execute
            try
            {
               var output = await command.ExecuteAsync(context);
               return CommandExecutionResult.Success(output);
            }
            catch (Exception e)
            {
                string commandName = command.GetType().Name;
                string message = $"Command '{commandName}' threw an exception during execution: {e.Message}";
                
                return CommandExecutionResult.Fail(message);
            }
        }
    }
}