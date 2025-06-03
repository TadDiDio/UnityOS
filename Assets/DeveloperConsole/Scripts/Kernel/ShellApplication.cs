using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace DeveloperConsole
{
    public abstract class ShellApplication : IKernelApplication
    {
        protected IInputManager InputManager { get; } = new BufferedInputManager();
        protected IOutputManager OutputManager { get; } = new OutputManager();
        
        private ICommandParser _commandParser;
        private ITokenizationManager _tokenizationManager;

        private Stack<REPLCommand> _repls = new();
        
        private bool _commandExecuting;
        
        protected ShellApplication(ITokenizationManager tokenizationManager, ICommandParser commandParser)
        {
            _tokenizationManager = tokenizationManager;
            _commandParser = commandParser;
        }

        protected abstract CommandContext GetSpecificContext();
        protected virtual void OnBeforeInputProcessed(string rawInput) { }
        protected virtual void OnAfterInputProcessed(CommandRunResult result) { }

        public async Task<string> WaitForInput()
        {
            if (InputManager is BufferedInputManager bufferedInputManager)
            {
                bufferedInputManager.ClearBuffer();
            }
            
            return await InputManager.WaitForInput();
        }
        
        public void SendOutput(string message)
        {
            OutputManager.SendOutput(message);
        }

        public void Tick()
        {
            // TODO: Tick any commands which need it

            // Only consume input if we can run it.
            if (_commandExecuting) return;
            if (InputManager.TryGetInput(out var input)) SubmitInput(input);
        }
        private async void SubmitInput(string input)
        {
            try
            {
                if (_commandExecuting) return;
                
                // TODO: Use user config instead of >
                OutputManager.SendOutput($"> {input}");

                OnBeforeInputProcessed(input);
                var result = await RunInput(input);
                OnAfterInputProcessed(result);

                OutputManager.SendOutput(GetOutputStringFromResult(result));
            }
            catch (Exception)
            {
                Debug.LogError("A shell had an unexpected error while running a command.");
            }
        }

        public string GetOutputStringFromResult(CommandRunResult result)
        {
            if (result.ParseResult.Error is not ParseError.None)
            {
                return ErrorLogging.ParserError(result.ParseResult);
            }
            
            if (!string.IsNullOrEmpty(result.PreValidationError))
            {
                return result.PreValidationError;
            }
            
            return result.CommandResult.Message;
        }
        
        public async Task<CommandRunResult> RunInput(string rawInput)
        {
            try
            {
                _commandExecuting = true;

                CommandRunResult result = new();
                
                // Tokenize
                result.TokenizationResult = _tokenizationManager.Tokenize(rawInput);
                if (!result.TokenizationResult.Success || !result.TokenizationResult.TokenStream.HasMore()) return result;

                // Parse
                result.ParseResult = _commandParser.Parse(result.TokenizationResult.TokenStream);
                if (result.ParseResult.Error is not ParseError.None) return result;

                // Build context
                CommandContext context = GetSpecificContext();
                context.Shell = this;
                context.Tokens = result.TokenizationResult.Tokens;

                // Pre-execution validation 
                foreach (var validator in result.ParseResult.Command.GetType().GetCustomAttributes<PreExecutionValidatorAttribute>())
                {
                    if (await validator.Validate(context)) continue;
                    
                    result.PreValidationError = validator.OnValidationFailedMessage();
                    return result;
                }

                if (result.ParseResult.Command is REPLCommand repl)
                {
                    _repls.Push(repl);
                }
                
                result.CommandResult = await result.ParseResult.Command.ExecuteAsync(context);
                
                if (result.ParseResult.Command is REPLCommand)
                {
                    _repls.Pop();
                }
                
                result.Success = true;
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return new CommandRunResult();
            }
            finally
            {
                _commandExecuting = false;
            }
        }
        
        public void Dispose()
        {
            InputManager.UnregisterAllInputSources();
            
            // TODO: Dispose any async commands which need it
        }

        public struct CommandRunResult
        {
            public bool Success;
            public string PreValidationError;
            public TokenizationResult TokenizationResult;
            public ParseResult ParseResult;
            public CommandResult CommandResult;
        }
    }
}