using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace DeveloperConsole
{
    public class Kernel : Singleton<Kernel>, IDisposable
    {
        public static event Action<Event> OnEventOccured;

        private ConsoleState _consoleState;
        private ConsoleRuntimeDependencies _dependencies;
        
        private readonly Dictionary<Type, object> _proxies = new();

        private List<ITickable> _tickableComponents = new();

        private TerminalGUI _terminalGUI;
        private KernelApplication _kernelApplication; 
        private Stack<ITerminalApplication> _terminalApplications = new();

        private static readonly Dictionary<Type, object> _serviceMap = new();
        
        #region SETUP
        public Kernel(ConsoleConfiguration config, ConsoleState consoleState)
        {
            _dependencies = config.Create();

            _serviceMap.Add(typeof(IWindowManager), _dependencies.WindowManager);
            _serviceMap.Add(typeof(IInputManager), _dependencies.InputManager);
            _serviceMap.Add(typeof(IOutputManager), _dependencies.OutputManager);
            _serviceMap.Add(typeof(IAutoRegistrationProvider), _dependencies.AutoRegistration);
            _serviceMap.Add(typeof(ITokenizationManager), _dependencies.TokenizationManager);
            _serviceMap.Add(typeof(ITypeParserRegistryProvider), _dependencies.TypeParserRegistry);
            _serviceMap.Add(typeof(ICommandRegistryProvider), _dependencies.CommandRegistry);
            _serviceMap.Add(typeof(IConsoleParser), _dependencies.ConsoleParser);
            
            _terminalGUI = new TerminalGUI();
            _consoleState = consoleState;
            _kernelApplication = new KernelApplication(_consoleState.OutputBuffer);
            _dependencies.InputManager.InputSubmitted += OnInput;
            
            _dependencies.InputManager.RegisterInputSource(_terminalGUI);
            _dependencies.OutputManager.RegisterOutputSink(_kernelApplication);
        }
        public void RegisterTickable(ITickable tickable)
        {
            if (_tickableComponents.Contains(tickable))
            {
                // TODO: Console error
                return; 
            }
            _tickableComponents.Add(tickable);
        }
        public void UnregisterTickable(ITickable tickable)
        {
            if (!_tickableComponents.Contains(tickable))
            {
                // TODO: Console error
                return; 
            }
            _tickableComponents.Remove(tickable);
        }
        #endregion

        // Client-facing: wraps services in dynamically created interfaces to avoid reference caching problems.
        // This ensures that when the kernel dies, so do all its systems even if clients cache references.
        public T Get<T>() where T : class
        {
            if (_dependencies == null) throw new ObjectDisposedException(nameof(Kernel), "Kernel has been disposed.");

            if (_proxies.TryGetValue(typeof(T), out var existing)) return (T)existing;

            // Create a transparent proxy for T backed by kernel
            var proxy = DispatchProxy.Create<T, KernelServiceProxy<T>>();
            ((KernelServiceProxy<T>)(object)proxy).Setup(this);

            _proxies.Add(typeof(T), proxy);
            return proxy;
        }

        // Called by proxies to get the live service instance from _deps
        public T GetLiveInstance<T>() where T : class
        {
            if (_dependencies == null) return null;

            if (_serviceMap.TryGetValue(typeof(T), out var service)) return (T)service;

            return null;
        }
        
        public void Tick()
        {
            foreach (var tickable in _tickableComponents) tickable.Tick();
        }

        public void OnGUI(int screenWidth, int screenHeight)
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
            _dependencies.WindowManager.OnGUI(context);
        }
        
        private void OnInput(string rawInput)
        {
            _consoleState.AddHistory(rawInput);
            
            ActiveTerminalApplication().OnInputRecieved(rawInput);
        }

        private ITerminalApplication ActiveTerminalApplication()
        {
            return _terminalApplications.Count > 0 ? _terminalApplications.Peek() : _kernelApplication;
        }

        public async void RunInput(string rawInput)
        {
            try
            {
                // TODO: Use user config instead of > and maybe don't always put raw input in bc
                // it doesnt make sense for things like files. This likely needs to be somewhere else
                _dependencies.OutputManager.SendOutput($"> {rawInput}");
                
                // Tokenize
                var tokenizationResult = _dependencies.TokenizationManager.Tokenize(rawInput);
                if (!tokenizationResult.Success || !tokenizationResult.TokenStream.HasMore()) return;

                // Parse
                var parseResult = _dependencies.ConsoleParser.Parse(tokenizationResult.TokenStream);
                if (parseResult.Error is not ParseError.None)
                {
                    _dependencies.OutputManager.SendOutput(ErrorLogging.ParserError(parseResult)); 
                    return;
                }
                
                // TODO: PreCommandAttribute 
                
                
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
                string message = $"The console had an unexpected error, most likely while running a command:" +
                                 $" {e.Message}{Environment.NewLine}{e.StackTrace}";
                Debug.LogError(message);
            }
        }

        private CommandArgsBase GetCommandArgs(bool isConsoleCommand, List<string> tokens)
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
        
        public void Dispose()
        {
            _dependencies = null;
            _proxies.Clear();
        }
    }
}

