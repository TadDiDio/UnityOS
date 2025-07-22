using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Executes commands.
    /// </summary>
    public class CommandExecutor : ICommandExecutor
    {
        public async Task<CommandExecutionResult> ExecuteCommand(CommandExecutionRequest request, CancellationToken cancellationToken)
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
            context.Environment = UnityEnvironment.Build;
#endif

            // 3. Pre-execution validation
            ICommand command = resolution.Command;
            var validators = command.GetType().GetCustomAttributes<PreExecutionValidatorAttribute>();

            foreach (var validator in validators)
            {
                if (await validator.Validate(context, cancellationToken)) continue;
                return CommandExecutionResult.Fail(validator.OnValidationFailedMessage());
            }

            // 4. Execute
            try
            {
                var output = await command.ExecuteAsync(context, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    output.Message =
                        MessageFormatter.Warning(
                            "Command exited without throwing after a cancellation was requested. ") + output.Message;
                }

                return CommandExecutionResult.Success(output);
            }
            catch (OperationCanceledException)
            {
                throw;
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
