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
        public static event Action<Event> OnEventOccured;

        private static ConsoleState _consoleState;

        private static List<ITickable> _tickableComponents = new();

        private static TerminalGUI _terminalGUI;
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
            OnEventOccured?.Invoke(Event.current);
            
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
                // Tokenize
                var tokenizationResult = TokenizationManager.Tokenize(rawInput);
                if (!tokenizationResult.Success || tokenizationResult.Tokens.Count == 0) return;

                // Parse
                var parseResult = ConsoleParser.Parse(tokenizationResult.Tokens);
                if (parseResult.Error is not ParseError.None)
                {
                    ConsoleOutputManager.SendOutput(ErrorLogging.ParserError(parseResult)); 
                    return;
                }
                
                // Run
                CommandArgsBase args = GetCommandArgs(parseResult.Command is ConsoleCommand, tokenizationResult.Tokens);
                var commandResult = await parseResult.Command.ExecuteAsync(args);
                
                // Send output
                // TODO: Move this to a callback after output is sent because async. Also block input while async
                // processing unless user executes in background.
                
                _consoleState.AddOutput(commandResult.Message);
                JsonFileManager.Save(_consoleState);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private static CommandArgsBase GetCommandArgs(bool isConsoleCommand, List<string> tokens)
        {
            CommandArgsBase args;
            
            if (isConsoleCommand)
            {
                args = new ConsoleCommandArgs
                {
                    ConsoleState = _consoleState
                };
            }
            else
            {
                args = new CommandArgs();
            }

            args.Tokens = tokens;
            return args;
        }
    }
}

