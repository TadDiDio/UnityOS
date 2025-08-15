using System;
using System.Collections.Generic;
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
        // TODO: Don't do this here, move it to a public registry so the API can add more.
        private List<ICommandValidator> _commandValidators = new()
        {
            new InstantiateVariadic(),
            new BindingProcessor()
        };

        public async Task<CommandExecutionResult> ExecuteCommand(ShellRequest request, IOContext ioContext, CancellationToken cancellationToken)
        {
            // 1. Resolve
            var resolution = request.CommandResolver.Resolve(request.Session, request.ExpandAliases);

            switch (resolution.Status)
            {
                case CommandResolutionStatus.Success: break;
                case CommandResolutionStatus.AliasExpansion:
                    return CommandExecutionResult.AliasExpansion(resolution.Tokens);
                case CommandResolutionStatus.Fail:
                    ioContext.Output.WriteLine(Formatter.Error(resolution.ErrorMessage));
                    return CommandExecutionResult.Fail();
                case CommandResolutionStatus.Cancelled: break;
                default: throw new ArgumentOutOfRangeException();
            }

            // 2. Build context
            var context = BuildContext(request, ioContext);

            // 3. Pre-execution wiring and validation
            ICommand command = resolution.Command;

            try
            {
                var validators = command.GetType().GetCustomAttributes<PreExecutionValidatorAttribute>();

                foreach (var validator in validators)
                {
                    if (await validator.Validate(context, cancellationToken)) continue;
                    ioContext.Output.WriteLine(Formatter.Error(validator.OnValidationFailedMessage()));
                    return CommandExecutionResult.Fail();
                }

                // 4. Command validators
                foreach (var cmdValidator in _commandValidators)
                {
                    if (cmdValidator.Validate(command, out var error)) continue;
                    ioContext.Output.WriteLine(Formatter.Error(error));
                    return CommandExecutionResult.Fail();
                }

                // 5. Execute
                var output = await command.ExecuteCommandAsync(context, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    output.Message = Formatter.Warning(
                        $"Command exited without throwing after a cancellation was requested. {output.Message}");
                }

                string message = output.Status is Status.Success ? output.Message : Formatter.Error(output.Message);
                ioContext.Output.WriteLine(message);

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
                ioContext.Output.WriteLine(Formatter.Error(message));

                return CommandExecutionResult.Fail();
            }
            finally
            {
                command.Dispose();
            }
        }

        private FullCommandContext BuildContext(ShellRequest request, IOContext ioContext)
        {
#if UNITY_EDITOR
            var environment = Application.isPlaying ? UnityEnvironment.PlayMode : UnityEnvironment.EditMode;
#else
            var environment = UnityEnvironment.Build;
#endif

            var context = new FullCommandContext
            (
                new WriteContext(ioContext.Output),
                new PromptContext(ioContext.Prompt),
                new SignalContext(ioContext.SignalEmitter),
                new FullSessionContext(request.Session.CommandSubmitter, request.Session.AliasManager),
                environment
            );
            return context;
        }
    }
}
