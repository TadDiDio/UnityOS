#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using DeveloperConsole.Core;

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
        private static RuntimeDependenciesFactory _configurationOverride;
        
        #region AUTO BOOTSTRAP
#if UNITY_EDITOR
        static KernelBootstrapper() => Bootstrap();
#endif
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void RuntimeBootstrap() => Bootstrap();
#endif
        #endregion
        
        
        /// <summary>
        /// Starts or restarts the developer console if it is already started.
        /// </summary>
        public static void Bootstrap()
        {
            KillSystem();
            KernelUpdater updater = new KernelUpdater();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (Application.isPlaying)
            {
                PlayModeTickerSpawner.Initialize(() => new PlayModeTickerSpawner(updater));
            }
#endif
#if UNITY_EDITOR
            EditModeTicker.Initialize(() => new EditModeTicker(updater));
#endif
            CommonBootstrap();
        }
        
        
        /// <summary>
        /// Shuts down the developer console system.
        /// </summary>
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
        
        
        /// <summary>
        /// Sets an override configuration to inject custom dependencies.
        /// </summary>
        /// <param name="config"></param>
        public static void SetConfigurationOverride(RuntimeDependenciesFactory config)
        {
            _configurationOverride = config;
        }
        
        private static void CommonBootstrap()
        {
            if (_commonElementsInitialized) return;
            
            RuntimeDependenciesFactory config = GetConfiguration();
            Kernel.Initialize(() => new Kernel(config));
            
            _commonElementsInitialized = true;
        }
        
        private static RuntimeDependenciesFactory GetConfiguration()
        {
            return _configurationOverride ?? new RuntimeDependenciesFactory();
        }
    }
}