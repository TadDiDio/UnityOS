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
        public async Task<CommandExecutionResult> ExecuteCommand(
            ShellRequest request,
            UserInterface userInterface,
            CancellationToken cancellationToken)
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
                Session = request.Session
            };
#if UNITY_EDITOR
            context.Environment = Application.isPlaying ? UnityEnvironment.PlayMode : UnityEnvironment.EditMode;
#else
            context.Environment = UnityEnvironment.Build;
#endif

            // 3. Pre-execution wiring and validation
            ICommand command = resolution.Command;

            try
            {
                context.CommandId = command.CommandId;
                request.Session.RegisterInterfaceId(context.CommandId, userInterface);

                var validators = command.GetType().GetCustomAttributes<PreExecutionValidatorAttribute>();

                foreach (var validator in validators)
                {
                    if (await validator.Validate(context, cancellationToken)) continue;
                    request.Session.WriteLine(context.CommandId, MessageFormatter.Error(validator.OnValidationFailedMessage()));
                    return CommandExecutionResult.Fail();
                }

                // 4. Execute
                var output = await command.ExecuteCommandAsync(context, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    output.Message = MessageFormatter.Warning
                    ("Command exited without throwing after a cancellation was requested. ") + output.Message;
                }

                string message = output.Status is Status.Success ? output.Message : MessageFormatter.Error(output.Message);
                userInterface.Output.WriteLine(message);

                return output.Status is Status.Success ? CommandExecutionResult.Success() : CommandExecutionResult.Fail();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                string commandName = command.GetType().Name;
                string message = $"Command '{commandName}' threw an exception during execution: {e.Message}";
                userInterface.Output.WriteLine(MessageFormatter.Error(message));

                return CommandExecutionResult.Fail();
            }
            finally
            {
                request.Session.UnregisterInterfaceId(context.CommandId);
                command.Dispose();
            }
        }
    }
}
