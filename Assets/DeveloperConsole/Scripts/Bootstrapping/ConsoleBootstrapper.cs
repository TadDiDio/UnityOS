#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using DeveloperConsole.Core.Kernel;

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
        public static event Action SystemInitialized;
        private static DependenciesFactory _configurationOverride;

        #region AUTO BOOTSTRAP
        // Handles bootstrapping while in the editor but not play mode.
#if UNITY_EDITOR
        // Called on domain reload
        static ConsoleBootstrapper()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode) Bootstrap();
        }
#endif
        // Handles bootstrapping when entering playmode
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Called when entering playmode
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
            InstallComponents();

            SystemInitialized?.Invoke();
        }


        /// <summary>
        /// Shuts down the developer console system.
        /// </summary>
        public static void KillSystem()
        {
            _configurationOverride = null;

            // Unload runners
            if (PlayModeTickerSpawner.IsInitialized)
            {
                PlayModeTickerSpawner.Instance.DestroySceneConsole();
                PlayModeTickerSpawner.Reset();
            }

#if UNITY_EDITOR
            if (EditModeTicker.IsInitialized)
            {
                EditModeTicker.Instance.Clear();
                EditModeTicker.Reset();
            }
#endif

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
        public static void SetConfigurationOverride(DependenciesFactory config)
        {
            _configurationOverride = config;
        }

        private static void CommonBootstrap()
        {
            DependenciesFactory config = GetConfiguration();
            Kernel.Initialize(() => new Kernel(config));
        }

        private static DependenciesFactory GetConfiguration()
        {
            return _configurationOverride ?? new DependenciesFactory();
        }

        private static IEnumerable<Type> SafeGetTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null)!;
            }
            catch
            {
                return Array.Empty<Type>();
            }
        }

        private static void InstallComponents()
        {
            var installers = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !LibIsKnownSafeToSkip(a))
                .SelectMany(SafeGetTypes)
                .Where(t => typeof(IConsoleInstaller).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass);

            foreach (var type in installers)
            {
                var instance = (IConsoleInstaller)Activator.CreateInstance(type)!;
                instance.Install();
            }
        }

        private static bool LibIsKnownSafeToSkip(Assembly a)
        {
            var name = a.GetName().Name;
            if (string.IsNullOrEmpty(name))
                return true;

            return name.StartsWith("UnityEngine")
                   || name.StartsWith("UnityEditor")
                   || name.StartsWith("System")
                   || name.StartsWith("mscorlib")
                   || name.StartsWith("netstandard")
                   || name.StartsWith("TextMeshPro")
                   || name.StartsWith("Mono.")
                   || name.StartsWith("Unity.")
                   || name.StartsWith("nunit")
                   || name.StartsWith("JetBrains")
                   || name.StartsWith("Microsoft.")
                   || name.StartsWith("Boo.")
                   || name.StartsWith("ExCSS") // Example 3rd party lib
                   || name.Contains("Newtonsoft"); // common JSON lib
        }
    }
}
