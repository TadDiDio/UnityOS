using UnityEngine;
using UnityEngine.UIElements;

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

            _console = new GameObject(GameObjectName);
            _console.AddComponent<PlayModeTicker>().SetUpdater(kernelUpdater);
            Object.DontDestroyOnLoad(_console);
        }

        /// <summary>
        /// Gets a UI Document to register windows to in game view.
        /// </summary>
        /// <returns>The document.</returns>
        public UIDocument GetUIDocument()
        {
            if (_console.TryGetComponent<UIDocument>(out var uidDocument)) return uidDocument;
            var result = _console.AddComponent<UIDocument>();
            result.panelSettings = Resources.Load<PanelSettings>("Panel Settings");
            result.visualTreeAsset = Resources.Load<VisualTreeAsset>("Window");
            return result;
        }

        /// <summary>
        /// Destroys the scene play mode ticker.
        /// </summary>
        public void DestroySceneConsole()
        {
            if (_console) Object.Destroy(_console);
        }
    }
}
