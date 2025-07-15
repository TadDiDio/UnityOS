using System;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Executes commands.
    /// </summary>
    public class CommandExecutor : ICommandExecutor
    {
        public async Task<CommandExecutionResult> ExecuteCommand(CommandExecutionRequest request)
        {
            // 1. Resolve
            var resolution = request.Resolver.Resolve(request.ShellSession);
            if (resolution.Status is not Status.Success)
            {
                return CommandExecutionResult.Fail(resolution.ErrorMessage);
            }

            // 2. Build context
            var context = new CommandContext
            {
                Shell = request.Shell,
                Session = request.ShellSession
            };
#if UNITY_EDITOR
            context.Environment = Application.isPlaying ? UnityEnvironment.PlayMode : UnityEnvironment.EditMode;
#else
            context.Environment = UnityEnvironment.BuildMode;
#endif

            // 3. Pre-execution validation
            ICommand command = resolution.Command;
            var validators = command.GetType().GetCustomAttributes<PreExecutionValidatorAttribute>();

            foreach (var validator in validators)
            {
                if (await validator.Validate(context)) continue;
                return CommandExecutionResult.Fail(validator.OnValidationFailedMessage());
            }

            // 4. Register any unique type parsers
            command.RegisterTypeParsers();

            // 5. Execute
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
