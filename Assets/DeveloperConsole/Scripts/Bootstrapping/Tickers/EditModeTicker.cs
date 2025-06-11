#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace DeveloperConsole
{
    /// <summary>
    /// Captures logical and graphical update events to forward to the kernel while in edit mode.
    /// Acts as the edit mode entry point for the system.
    /// </summary>
    public class EditModeTicker : Singleton<EditModeTicker>
    {
        private const int ToolbarHeight = 25;

        private bool _disposed;
        private KernelUpdater _updater;
        
        public EditModeTicker(KernelUpdater updater)
        {
            _updater = updater;
            
            EditorApplication.update += OnTick;
            SceneView.duringSceneGui += OnGUI;
            AssemblyReloadEvents.beforeAssemblyReload += Clear;
        }

        public void Clear()
        {
            _disposed = true;
            EditorApplication.update -= OnTick;
            SceneView.duringSceneGui -= OnGUI;
        }

        private void OnTick()
        {
            // Unity holds onto delegates until next update loop so there is a single frame AFTER clear
            // is called where this can still be invoked. Fricken Unity man...
            if (_disposed) return;
            
            _updater.Tick();
        }
            
        
        private void OnGUI(SceneView sceneView)
        {
            if (_disposed) return;
            
            if (!Kernel.IsInitialized)
            {
                Debug.LogWarning("Kernel not initialized, yet edit mode ticker is. This should not happen. Skipping OnGUI.");
                return;
            }
            
            int width = (int)sceneView.position.width;
            int height = (int)sceneView.position.height - ToolbarHeight;
            
            _updater.Input(Event.current);
            _updater.Draw(width, height, true);
            sceneView.Repaint();
        }
    }
}
#endif