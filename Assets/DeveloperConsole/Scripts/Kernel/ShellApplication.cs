using System;
using System.Threading.Tasks;
using UnityEngine;

namespace DeveloperConsole
{
    public abstract class ShellApplication : IKernelApplication
    {
        protected IInputManager InputManager { get; } = new InputManager();
        protected IOutputManager OutputManager { get; } = new OutputManager();
        
        private IConsoleParser _consoleParser;
        private ITokenizationManager _tokenizationManager;

        protected ShellApplication(ITokenizationManager tokenizationManager, IConsoleParser consoleParser)
        {
            _tokenizationManager = tokenizationManager;
            _consoleParser = consoleParser;
            InputManager.InputSubmitted += SubmitInput;
        }

        protected virtual void OnBeforeInputProcessed(string rawInput) { }
        protected virtual void OnAfterInputProcessed(CommandResult result) { }

        private async void SubmitInput(string input)
        {
            try
            {
                await RunInput(input);
            }
            catch (Exception)
            {
                // Ignored
            }
        }
        protected virtual async Task RunInput(string rawInput, ConsoleState consoleState = null)
        {
            try
            {
                OnBeforeInputProcessed(rawInput);
                OnBeforeInputProcessed(rawInput);
                // TODO: Use user config instead of > and maybe don't always put raw input
                OutputManager.SendOutput($"> {rawInput}");
                
                // Tokenize
                var tokenizationResult = _tokenizationManager.Tokenize(rawInput);
                if (!tokenizationResult.Success || !tokenizationResult.TokenStream.HasMore()) return;

                // Parse
                var parseResult = _consoleParser.Parse(tokenizationResult.TokenStream);
                if (parseResult.Error is not ParseError.None)
                {
                    OutputManager.SendOutput(ErrorLogging.ParserError(parseResult)); 
                    return;
                }
                
                // TODO: PreCommandAttribute 
                
                // Run
                if (consoleState == null) consoleState = ConsoleState.Default();
                CommandContext context = new()
                {
                    Tokens = tokenizationResult.Tokens,
                    ConsoleState = consoleState
                };
                
                var commandResult = await parseResult.Command.ExecuteAsync(context);
                
                OutputManager.SendOutput(commandResult.Message);
                OnAfterInputProcessed(commandResult);
            }
            catch (Exception e)
            {
                Debug.LogError($"The console had an unexpected error, most likely while running a command: {e.Message}{Environment.NewLine}{e.StackTrace}");
            }
        }
        
        public void Tick()
        {
            // TODO: Tick any commands which need it
        }
        
        public void Dispose()
        {
            InputManager.InputSubmitted -= SubmitInput;
            InputManager.UnregisterAllInputSources();
            
            // TODO: Dispose any commands which need it
        }
    }
}