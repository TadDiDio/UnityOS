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
        private static List<IGraphical> _graphicalComponents = new();

        private static KernelGUI _kernelGUI = new();
        private static ConsoleState _consoleState;

        private static DefaultTerminalOutput _defaultTerminalOutput; 
        private static Stack<ITerminalApplication> _terminalApplications = new();

        private static ITokenizer _tokenizer = new DefaultTokenizer();
        
        #region SETUP
        static ConsoleKernel()
        {
            ConsoleInputManager.InputSubmitted += OnInput;
            
#if UNITY_EDITOR
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
#endif
            
            _consoleState = JsonFileManager.Load();
            _defaultTerminalOutput = new DefaultTerminalOutput(_consoleState.OutputBuffer);
            
            // TODO: Setup default pipeline here, prob move this to proper initialization later
            ConsoleInputManager.RegisterInputMethod(_kernelGUI);
            ConsoleOutputManager.RegisterOutputSink(new DefaultTerminalOutput(_consoleState.OutputBuffer));
        }
        
#if UNITY_EDITOR
        private static void OnBeforeAssemblyReload()
        {
            ConsoleInputManager.InputSubmitted -= OnInput;
            ConsoleInputManager.UnregisterAllInputMethods();
            ConsoleOutputManager.UnregisterAllOutputSinks();
        }
#endif
        
        public static void Register(object component)
        {
            if (component is ITickable tickable)
            {
                if (_tickableComponents.Contains(tickable))
                {
                    // TODO: Console error
                }
                else _tickableComponents.Add(tickable);
            }
            
            if (component is IGraphical graphical)
            {
                if (_graphicalComponents.Contains(graphical))
                {
                    // TODO: Console error
                }
                else _graphicalComponents.Add(graphical);
            }
        }
        public static void Unregister(object component)
        {
            if (component is ITickable tickable)
            {
                if (!_tickableComponents.Contains(tickable))
                {
                    // TODO: Console error
                }
                else _tickableComponents.Remove(tickable);
            }
            
            if (component is IGraphical graphical)
            {
                if (!_graphicalComponents.Contains(graphical))
                {
                    // TODO: Console error
                }
                else _graphicalComponents.Remove(graphical);
            }
        }
        
        #endregion
        
        public static void Tick()
        {
            foreach (var tickable in _tickableComponents) tickable.Tick();
        }

        public static void OnGUI(int screenWidth, int screenHeight)
        {
            // TODO: Move this padding to config
            int padding = 10;
            Rect screenRect = new(padding, padding, screenWidth - 2 * padding, screenHeight - 2 * padding);
            GUIContext context = new GUIContext
            {
                AreaRect = screenRect,
                Style = _consoleState.ConsoleStyle
            };
            
            _kernelGUI.Draw(context, ActiveTerminalApplication());
            
            foreach (var graphical in _graphicalComponents) graphical.OnGUI(context);
            OnEvent?.Invoke(Event.current);
        }
        
        private static void OnInput(string rawInput)
        {
            _consoleState.AddHistory(rawInput);
            
            ActiveTerminalApplication().OnInput(rawInput);
        }

        private static ITerminalApplication ActiveTerminalApplication()
        {
            return _terminalApplications.Count > 0 ? _terminalApplications.Peek() : _defaultTerminalOutput;
        }

        public static async void RunInput(string rawInput)
        {
            try
            {
                if (!TokenizeAndValidate(rawInput, out var tokenizationResult)) return;

                var parseResult = Parser.Parse(tokenizationResult.Tokens);
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
            result = _tokenizer.Tokenize(rawInput);
            if (!result.Success) return false;
            return result.Tokens.Count >= 1;
        }
        
        private static void Print(object obj)
        {
            Debug.Log(obj.ToString());
        }
    }
}

