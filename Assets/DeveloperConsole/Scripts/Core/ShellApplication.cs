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
        private IInputManager _inputManager;
        private IOutputManager _outputManager;
        
        
        /// <summary>
        /// Creates a new shell application.
        /// </summary>
        /// <param name="executor">The command executor.</param>
        /// <param name="inputManager">The input maanger.</param>
        /// <param name="outputManager">The output manager.</param>
        public ShellApplication(ICommandExecutor executor, IInputManager inputManager, IOutputManager outputManager)
        {
            _executor = executor;
            _inputManager = inputManager;
            _outputManager = outputManager;
            _inputManager.OnCommandInput += HandleCommandRequestAsync;
        }
        
        // TODO: 
        public ShellSession CreateSession()
        {
            return null;
        }

        
        public async void HandleCommandRequestAsync(CommandRequest request)
        {
            try
            {
                // 1. Execute
                CommandExecutionRequest executionRequest = new()
                {
                    Request = request,
                    Output = _outputManager,
                };
                
                var executionResult = await _executor.ExecuteCommand(executionRequest);
                
                // 2. Output
                IOutputMessage output;
                if (executionResult.Status is not Status.Success)
                {
                    output = new SimpleOutputMessage(request.ShellSession, executionResult.ErrorMessage);
                }
                else
                {
                    output = new ShellOutputMessage
                    (
                        request.ShellSession,
                        executionResult.CommandOutput
                    );
                }
                
                _outputManager.Emit(output);
            }
            catch (Exception e)
            {
                Log.Error($"Shell had an unexpected error while executing: {e}");
            }
        }
        
        public void Tick() { /* TODO */ }

        public void Dispose()
        {
            _inputManager.OnCommandInput -= HandleCommandRequestAsync;
        }
    }
}