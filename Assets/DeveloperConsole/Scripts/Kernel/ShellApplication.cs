using System;
using System.Collections.Generic;
using System.Linq;
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

        private Stack<(ReplCommand repl, bool executingCommand)> _repls = new();
        
        private bool _commandExecuting;
        
        protected ShellApplication(ITokenizationManager tokenizationManager, ICommandParser commandParser)
        {
            _tokenizationManager = tokenizationManager;
            _commandParser = commandParser;
        }

        protected abstract CommandContext GetSpecificCommandContext();
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
        
        public void SendOutput(string message, bool isUserInput)
        {
            string output = isUserInput ? GetPrompt() + message : message;
            OutputManager.SendOutput(output);
        }

        public void Tick()
        {
            // TODO: Tick any commands which need it

            // Only consume input if we can run it.
            if (_commandExecuting && _repls.Count == 0) return;
            if (InputManager.TryGetInput(out var input)) HandleNewInput(input);
        }
        private async void HandleNewInput(string input)
        {
            try
            {
                if (_repls.Count > 0)
                {
                    _repls.Peek().repl.OnInput(input);
                    return;
                }

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

            return result.Empty ? "" : result.CommandResult.Message;
        }
        
        public async Task<CommandRunResult> RunInput(string rawInput)
        {
            try
            {
                _commandExecuting = true;

                CommandRunResult result = new()
                {
                    TokenizationResult = _tokenizationManager.Tokenize(rawInput),
                    Empty = true
                };

                if (!result.TokenizationResult.Success || !result.TokenizationResult.TokenStream.HasMore()) return result;

                result.Empty = false;
                
                // Parse
                result.ParseResult = await _commandParser.Parse(result.TokenizationResult.TokenStream);
                if (result.ParseResult.Error is not ParseError.None) return result;

                // Build context
                CommandContext context = GetSpecificCommandContext();
                context.Shell = this;
                context.Tokens = result.TokenizationResult.Tokens;

                // Pre-execution validation 
                foreach (var validator in result.ParseResult.Command.GetType().GetCustomAttributes<PreExecutionValidatorAttribute>())
                {
                    if (await validator.Validate(context)) continue;
                    
                    result.PreValidationError = validator.OnValidationFailedMessage();
                    return result;
                }

                if (result.ParseResult.Command is ReplCommand repl)
                {
                    _repls.Push((repl, false));
                }
                
                result.CommandResult = await result.ParseResult.Command.ExecuteAsync(context);
                
                if (result.ParseResult.Command is ReplCommand)
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

        protected string GetPrompt()
        {
            var prompts = _repls.Select(pair => pair.repl.GetPromptLabel()).ToList();

            prompts.Reverse();
            
            return string.Join(".", prompts) + "> ";
        }
        
        public void Dispose()
        {
            InputManager.UnregisterAllInputSources();
            
            // TODO: Dispose any async commands which need it
        }

        public struct CommandRunResult
        {
            public bool Success;
            public bool Empty;
            public string PreValidationError;
            public TokenizationResult TokenizationResult;
            public ParseResult ParseResult;
            public CommandResult CommandResult;
        }
    }
}