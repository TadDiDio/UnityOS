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
    public static class KernelBootstrapper
    {
        private static bool _commonElementsInitialized;
        private static ConsoleConfiguration _configurationOverride;
        
        public static void Bootstrap()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (Application.isPlaying) RuntimeBootstrap();
#endif
            
#if UNITY_EDITOR
            EditorBootstrap();
#endif
        }
        public static void KillSystem()
        {
            _configurationOverride = null;
            _commonElementsInitialized = false;
            
            // Unload runners
            if (PlayModeTickerSpawner.IsInitialized)
            {
                PlayModeTickerSpawner.Instance.DestroySceneConsole();
                PlayModeTickerSpawner.Reset();
            }

            if (EditModeTicker.IsInitialized)
            {
                EditModeTicker.Instance.Clear();
                EditModeTicker.Reset();
            }

            // Unload kernel last so that nothing is referencing it
            if (Kernel.IsInitialized)
            {
                Kernel.Instance.Dispose();
                Kernel.Reset();
            }
        }
        
#if UNITY_EDITOR
        static KernelBootstrapper() => EditorBootstrap();
        private static void EditorBootstrap()
        {
            KillSystem();
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
            KillSystem();
#if UNITY_EDITOR
            EditorBootstrap();
#else
            CommonBootstrap();
#endif            
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
            Kernel.Initialize(() => new Kernel(config));
            
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