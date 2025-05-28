#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

namespace DeveloperConsole
{
    /// <summary>
    /// Entry point of the entire console system in both edit and play mode.
    /// </summary>
    #if UNITY_EDITOR
    [InitializeOnLoad]
    #endif
    public abstract class ConsoleBootstrapper
    {
        private static bool _commonElementsInitialized;
        
#if UNITY_EDITOR
        static ConsoleBootstrapper() => EditorBootstrap();
        private static void EditorBootstrap()
        {
            CommonBootstrap();
            EditModeTicker.Initialize();
        }
#endif
     
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void RuntimeBootstrap()
        {
            // TODO: Only do this if the user wants and if it is URP
            //DebugManager.instance.enableRuntimeUI = false;
            
            CommonBootstrap();
            PlayModeTickerSpawner.SpawnConsole();
        }
        
        private static void CommonBootstrap()
        {
            if (_commonElementsInitialized) return;
            
            ConsoleConfiguration config = ConsoleConfigLoader.GetConsoleConfigurationSelector().SelectedConfiguration;

            if (!config) config = ConsoleConfiguration.Default();
            
            CommandRegistry.SetCommands(AutoRegistration.CommandTypes());
            InitializeTypeParsers(config.AutoDetectSetup);
            
            // Load console state
            ConsoleState consoleState = JsonFileManager.Load();
            
            // Create and register terminal
            TerminalGUI terminalGUI = new TerminalGUI();
            TerminalOutput terminalOutput = new TerminalOutput(consoleState.OutputBuffer);
            ConsoleInputManager.RegisterInputMethod(terminalGUI);
            ConsoleOutputManager.RegisterOutputSink(terminalOutput);

            // Initialize kernel
            ConsoleKernel.Initialize(consoleState, terminalGUI, terminalOutput);
            
            _commonElementsInitialized = true;
        }

        private static void InitializeTypeParsers(bool autoDetect)
        {
            TypeParserRegistry.RegisterTypeParser<int>(new IntParser());;
            TypeParserRegistry.RegisterTypeParser<float>(new FloatParser());
            TypeParserRegistry.RegisterTypeParser<string>(new StringParser());
            TypeParserRegistry.RegisterTypeParser<bool>(new BoolParser());
            TypeParserRegistry.RegisterTypeParser<Vector2>(new Vector2Parser());
            TypeParserRegistry.RegisterTypeParser<Vector3>(new Vector3Parser());
            TypeParserRegistry.RegisterTypeParser<Color>(new ColorParser());
            TypeParserRegistry.RegisterTypeParser<Color>(new AlphaColorParser());

            if (!autoDetect) return;
            
            // Auto-detect and add parsers
            foreach (var (parsedType, parserType) in AutoRegistration.TypeParsersTypes())
            {
                try
                {
                    var instance = Activator.CreateInstance(parserType);
                    var registerMethod = typeof(TypeParserRegistry)
                        .GetMethod("RegisterTypeParser")
                        ?.MakeGenericMethod(parsedType);

                    if (registerMethod != null) registerMethod.Invoke(null, new[] { instance });
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Failed to register parser {parserType.Name} for {parsedType.Name}: {e.Message}");
                }
            }
        }
    }
}