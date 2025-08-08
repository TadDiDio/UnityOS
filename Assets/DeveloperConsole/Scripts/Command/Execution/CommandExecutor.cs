using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Core.Shell;
using UnityEngine;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Executes commands.
    /// </summary>
    public class CommandExecutor : ICommandExecutor
    {
        public async Task<CommandExecutionResult> ExecuteCommand(ShellRequest request, CancellationToken cancellationToken)
        {
            // 1. Resolve
            var resolution = request.CommandResolver.Resolve(request.Session, request.ExpandAliases);

            switch (resolution.Status)
            {
                case CommandResolutionStatus.Success: break;
                case CommandResolutionStatus.AliasExpansion:
                    return CommandExecutionResult.AliasExpansion(resolution.Tokens);
                case CommandResolutionStatus.Fail:
                    return CommandExecutionResult.Fail(resolution.ErrorMessage);
            }

            // 2. Build context
            var context = new CommandContext
            {
                Shell = request.Shell,
                Session = request.Session
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
                    output.Message = MessageFormatter.Warning
                    ("Command exited without throwing after a cancellation was requested. ") + output.Message;
                }

                return output.Status is Status.Success ? CommandExecutionResult.Success(output) : CommandExecutionResult.Fail(output.Message);
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
            finally
            {
                command.Dispose();
            }
        }
    }
}
