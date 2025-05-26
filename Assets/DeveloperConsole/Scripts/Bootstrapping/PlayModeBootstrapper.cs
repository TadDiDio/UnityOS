using UnityEngine;

namespace DeveloperConsole
{
 
    /// <summary>
    /// Spawns an observer in play mode to forward logical and gui events to the kernel.
    /// Acts as the play mode entry point for the system.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class PlayModeBootstrapper : MonoBehaviour
    {
        private static bool _initialized;
        private const string GameObjectName = "[Developer Console]";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void SpawnConsole()
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (_initialized) return;
            
            // TODO: Only do this if the user wants and if it is URP
            //DebugManager.instance.enableRuntimeUI = false;
            
            var existing = FindObjectsByType<PlayModeTicker>(FindObjectsSortMode.None);
            
            if (existing.Length > 0)
            {
                for (int i = existing.Length - 1; i >= 0; i--)
                {
                    Destroy(existing[i].gameObject);
                }
            }
            
            GameObject console = new GameObject(GameObjectName);
            console.AddComponent<PlayModeTicker>();
            DontDestroyOnLoad(console);
            
            _initialized = true;
            #endif
        }
    }
}