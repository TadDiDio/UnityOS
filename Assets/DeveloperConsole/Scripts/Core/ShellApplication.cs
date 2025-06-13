using System;
using DeveloperConsole.IO;
using DeveloperConsole.Command;

namespace DeveloperConsole.Core
{
    public sealed class ShellApplication : IShellApplication
    {
        private ICommandExecutor _executor;
        private IInputManager _inputManager;
        private IOutputManager _outputManager;
        
        protected ShellApplication(ICommandExecutor executor, IInputManager inputManager, IOutputManager outputManager)
        {
            _executor = executor;
            _inputManager = inputManager;
            _outputManager = outputManager;
            _inputManager.OnCommandInput += HandleCommandRequestAsync;
        }
        
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
                    output = new SimpleOutputMessage(executionResult.ErrorMessage);
                }
                else
                {
                    output = new ShellOutputMessage
                    {
                        CommandOutput = executionResult.CommandOutput,
                        Session = request.ShellSession,
                        // TODO: Add channel info.
                    };
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