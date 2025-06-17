using System;
using DeveloperConsole.IO;
using DeveloperConsole.Command;

namespace DeveloperConsole.Core
{
    /// <summary>
    /// Routes input to the command executor and execution results to the output manager.
    /// </summary>
    public sealed class ShellApplication : IShellApplication
    {
        private ICommandExecutor _executor;
        public IInputManager InputManager { get; }
        public IOutputManager OutputManager { get; }
        
        /// <summary>
        /// Creates a new shell application.
        /// </summary>
        /// <param name="executor">The command executor.</param>
        /// <param name="inputManager">The input manager.</param>
        /// <param name="outputManager">The output manager.</param>
        public ShellApplication(ICommandExecutor executor, IInputManager inputManager, IOutputManager outputManager)
        {
            _executor = executor;
            InputManager = inputManager;
            OutputManager = outputManager;
            InputManager.OnCommandInput += HandleCommandRequestAsync;
        }
        
        
        // TODO:
        public ShellSession CreateSession()
        {
            return new ShellSession();
        }

        
        public async void HandleCommandRequestAsync(CommandRequest request)
        {
            try
            {
                // 1. Execute
                CommandExecutionRequest executionRequest = new()
                {
                    Request = request,
                    Shell = this,
                };
                
                var executionResult = await _executor.ExecuteCommand(executionRequest);
                
                // 2. Output result
                IOutputMessage output;
                if (executionResult.Status is not Status.Success)
                {
                    output = string.IsNullOrWhiteSpace(executionResult.ErrorMessage) ?
                        null :
                        new SimpleOutputMessage(request.ShellSession, executionResult.ErrorMessage);
                }
                else
                {
                    output = string.IsNullOrWhiteSpace(executionResult.CommandOutput.Message) ?
                    null :
                    new ShellOutputMessage
                    (
                        request.ShellSession,
                        executionResult.CommandOutput
                    );
                }

                if (output is not null)
                {
                    OutputManager.Emit(output);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Shell had an unexpected error while executing: {e}");
            }
        }
        
        public void Tick() { /* TODO */ }

        public void Dispose()
        {
            InputManager.OnCommandInput -= HandleCommandRequestAsync;
        }
    }
}