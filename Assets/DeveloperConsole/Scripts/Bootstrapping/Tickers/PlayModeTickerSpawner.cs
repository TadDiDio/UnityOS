using UnityEngine;

namespace DeveloperConsole
{
 
    /// <summary>
    /// Spawns an observer in play mode to forward logical and gui events to the kernel.
    /// Acts as the play mode entry point for the system.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public static class PlayModeTickerSpawner
    {
        private static bool _initialized;
        private const string GameObjectName = "[Developer Console]";

        public static void SpawnConsole()
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (_initialized) return;
            
            var existing = Object.FindObjectsByType<PlayModeTicker>(FindObjectsSortMode.None);
            
            if (existing.Length > 0)
            {
                for (int i = existing.Length - 1; i >= 0; i--)
                {
                    Object.Destroy(existing[i].gameObject);
                }
            }
            
            GameObject console = new GameObject(GameObjectName);
            console.AddComponent<PlayModeTicker>();
            Object.DontDestroyOnLoad(console);
            
            _initialized = true;
            #endif
        }
    }
}