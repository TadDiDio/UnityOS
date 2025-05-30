#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace DeveloperConsole
{
    /// <summary>
    /// Entry point of the entire console system in both edit and play mode.
    /// </summary>
    #if UNITY_EDITOR
    [InitializeOnLoad]
    #endif
    public static class ConsoleBootstrapper
    {
        private static bool _commonElementsInitialized;
        private static ConsoleConfiguration _configurationOverride;
        
        public static void Bootstrap()
        {
            if (Application.isPlaying)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                RuntimeBootstrap();
#endif
            }
            else
            {
#if UNITY_EDITOR
                EditorBootstrap();
#endif
            }
        }
        public static void ResetConsole()
        {
            _configurationOverride = null;
            _commonElementsInitialized = false;

            Kernel.Instance.Dispose();
            Kernel.Reset();
            
            // Unload runners
            PlayModeTickerSpawner.Instance.DestroySceneConsole();
            EditModeTicker.Reset();
            PlayModeTickerSpawner.Reset();
        }
        
#if UNITY_EDITOR
        static ConsoleBootstrapper() => EditorBootstrap();
        private static void EditorBootstrap()
        {
            CommonBootstrap();
            EditModeTicker.Initialize(() => new EditModeTicker());
        }
#endif
     
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void RuntimeBootstrap()
        {
            // TODO: Only do this if the user wants and if it is URP
            //DebugManager.instance.enableRuntimeUI = false;
            CommonBootstrap();
            PlayModeTickerSpawner.Initialize(() => new PlayModeTickerSpawner());
        }
#endif

        public static void SetConfigurationOverride(ConsoleConfiguration config)
        {
            _configurationOverride = config;
        }
        
        private static void CommonBootstrap()
        {
            if (_commonElementsInitialized) return;
            
            ConsoleConfiguration config = GetConfiguration();
            
            ConsoleState consoleState = JsonFileManager.Load();
            
            Kernel.Initialize(() => new Kernel(config, consoleState));
            
            _commonElementsInitialized = true;
        }

        private static ConsoleConfiguration GetConfiguration()
        {
            if (_configurationOverride != null) return _configurationOverride;
            
            ConsoleConfiguration config = ConsoleConfigLoader.GetConsoleConfigurationSelector().SelectedConfiguration;
            if (config == null) config = new ConsoleConfiguration();
            return config;
        }
    }
}