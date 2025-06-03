using UnityEngine;

namespace DeveloperConsole
{
 
    /// <summary>
    /// Spawns an observer in play mode to forward logical and gui events to the kernel.
    /// Acts as the play mode entry point for the system.
    /// </summary>
    public class PlayModeTickerSpawner : Singleton<PlayModeTickerSpawner>
    {
        private const string GameObjectName = "[Developer Console]";
        private GameObject _console;

        public PlayModeTickerSpawner(KernelUpdater kernelUpdater)
        {
            var existing = Object.FindObjectsByType<PlayModeTicker>(FindObjectsSortMode.None);
            
            if (existing.Length > 0)
            {
                for (int i = existing.Length - 1; i >= 0; i--)
                {
                    Object.Destroy(existing[i].gameObject);
                }
            }
            
            GameObject console = new GameObject(GameObjectName);
            console.AddComponent<PlayModeTicker>().SetUpdater(kernelUpdater);
            Object.DontDestroyOnLoad(console);
        }

        public void DestroySceneConsole()
        {
            if (_console) Object.Destroy(_console);
        }
    }
}