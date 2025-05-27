#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using UnityEngine;
using System.Collections.Generic;

namespace DeveloperConsole
{
    public static class ConsoleKernel
    {
        public static event Action<Event> OnEvent;

        private static List<ITickable> _tickableComponents = new();

        private static TerminalGUI _terminalGUI;
        private static ConsoleState _consoleState;

        private static TerminalOutput _terminalOutput; 
        private static Stack<ITerminalApplication> _terminalApplications = new();

        #region SETUP
        public static void Initialize(ConsoleState consoleState, TerminalGUI terminalGUI, TerminalOutput terminalOutput)
        {
#if UNITY_EDITOR
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
#endif

            _terminalGUI = terminalGUI;
            _consoleState = consoleState;
            _terminalOutput = terminalOutput;
            
            ConsoleInputManager.InputSubmitted += OnInput;
        }
        
#if UNITY_EDITOR
        private static void OnBeforeAssemblyReload()
        {
            ConsoleInputManager.InputSubmitted -= OnInput;
            ConsoleInputManager.UnregisterAllInputMethods();
            ConsoleOutputManager.UnregisterAllOutputSinks();
        }
#endif
        
        public static void RegisterTickable(ITickable tickable)
        {
            if (_tickableComponents.Contains(tickable))
            {
                // TODO: Console error
                return; 
            }
            _tickableComponents.Add(tickable);
        }
        public static void UnregisterTickable(ITickable tickable)
        {
            if (!_tickableComponents.Contains(tickable))
            {
                // TODO: Console error
                return; 
            }
            _tickableComponents.Remove(tickable);
        }
        
        #endregion
        
        public static void Tick()
        {
            foreach (var tickable in _tickableComponents) tickable.Tick();
        }

        public static void OnGUI(int screenWidth, int screenHeight)
        {
            OnEvent?.Invoke(Event.current);
            
            // TODO: Move this padding to config
            int padding = 10;
            Rect screenRect = new(padding, padding, screenWidth - 2 * padding, screenHeight - 2 * padding);
            GUIContext context = new GUIContext
            {
                AreaRect = screenRect,
                Style = _consoleState.ConsoleStyle
            };
            
            _terminalGUI.Draw(context, ActiveTerminalApplication());
            GraphicsManager.OnGUI(context);
        }
        
        private static void OnInput(string rawInput)
        {
            _consoleState.AddHistory(rawInput);
            
            ActiveTerminalApplication().OnInput(rawInput);
        }

        private static ITerminalApplication ActiveTerminalApplication()
        {
            return _terminalApplications.Count > 0 ? _terminalApplications.Peek() : _terminalOutput;
        }

        public static async void RunInput(string rawInput)
        {
            try
            {
                if (!TokenizeAndValidate(rawInput, out var tokenizationResult)) return;

                var parseResult = ConsoleParser.Parse(tokenizationResult.Tokens);
                if (parseResult.Error is not ParseError.None)
                {
                    // TODO: Output error
                    ConsoleOutputManager.SendOutput(ErrorLogging.ParserError(parseResult));
                    return;
                }
                
                
                // Run
                ConsoleCommandArgs args = new()
                {
                    Tokens = tokenizationResult.Tokens,
                    ConsoleState = _consoleState
                };
                    
                var commandResult = await parseResult.Command.ExecuteAsync(args);
                
                // Send output
            
                // TODO: Move this to a callback after output is sent and use correct arg
                _consoleState.AddOutput(commandResult.Message);
                JsonFileManager.Save(_consoleState);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private static bool TokenizeAndValidate(string rawInput, out TokenizationResult result)
        {
            // TODO Move this all to manager
            result = TokenizationManager.Tokenizer.Tokenize(rawInput);
            if (!result.Success) return false;
            return result.Tokens.Count >= 1;
        }
    }
}

