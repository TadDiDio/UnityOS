using System;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace DeveloperConsole
{
    public abstract class ShellApplication : IKernelApplication
    {
        protected IInputManager InputManager { get; } = new InputManager();
        protected IOutputManager OutputManager { get; } = new OutputManager();
        
        private ICommandParser _commandParser;
        private ITokenizationManager _tokenizationManager;

        private bool _inputLocked;
        
        protected ShellApplication(ITokenizationManager tokenizationManager, ICommandParser commandParser)
        {
            _tokenizationManager = tokenizationManager;
            _commandParser = commandParser;
            InputManager.InputSubmitted += SubmitInput;
        }

        protected virtual void OnBeforeInputProcessed(string rawInput) { }
        protected virtual void OnAfterInputProcessed(CommandResult result) { }

        private async void SubmitInput(string input)
        {
            if (_inputLocked) return;
            
            try
            {
                await RunInput(input);
            }
            catch (Exception)
            {
                Debug.LogError("A shell had an unexpected error, most likely while running a command.");
            }
        }

        protected abstract CommandContext GetSpecificContext();
        private async Task RunInput(string rawInput)
        {
            try
            {
                _inputLocked = true;
                OnBeforeInputProcessed(rawInput);
                
                // TODO: Use user config instead of >
                OutputManager.SendOutput($"> {rawInput}");
                
                // Tokenize
                var tokenizationResult = _tokenizationManager.Tokenize(rawInput);
                if (!tokenizationResult.Success || !tokenizationResult.TokenStream.HasMore()) return;
                
                // Parse
                var parseResult = _commandParser.Parse(tokenizationResult.TokenStream);
                if (parseResult.Error is not ParseError.None)
                {
                    OutputManager.SendOutput(ErrorLogging.ParserError(parseResult));
                    return;
                }
                
                // Build context
                CommandContext context = GetSpecificContext();
                context.Shell = this;
                context.Tokens = tokenizationResult.Tokens;
                
                // Pre-execution validation 
                foreach (var validator in parseResult.Command.GetType().GetCustomAttributes<PreExecutionValidatorAttribute>())
                {
                    bool result = await validator.Validate(context);

                    if (result) OutputManager.SendOutput(validator.OnValidationSucceededMessage());
                    else
                    {
                        OutputManager.SendOutput(validator.OnValidationFailedMessage());
                        return;
                    }
                }

                // Run 
                var commandResult = await parseResult.Command.ExecuteAsync(context);
                OnAfterInputProcessed(commandResult);
                
                OutputManager.SendOutput(commandResult.Message);
            }
            finally
            {
                _inputLocked = false;
            }
        }
        
        public async Task<string> WaitForInput(string message)
        {
            var source = new TaskCompletionSource<string>();

            OutputManager.SendOutput(message);
            InputManager.InputSubmitted += Handler;
            
            return await source.Task;

            void Handler(string input)
            {
                InputManager.InputSubmitted -= Handler;
                source.TrySetResult(input);
            }
        }
        
        public void SendOutput(string message)
        {
            OutputManager.SendOutput(message);
        }
        
        public void Tick()
        {
            // TODO: Tick any commands which need it
        }
        
        public void Dispose()
        {
            InputManager.InputSubmitted -= SubmitInput;
            InputManager.UnregisterAllInputSources();
            
            // TODO: Dispose any async commands which need it
        }
    }
}