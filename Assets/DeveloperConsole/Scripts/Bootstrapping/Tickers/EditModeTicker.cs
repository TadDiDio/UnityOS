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
        private bool _disposed;
        private KernelUpdater _updater;

        public EditModeTicker(KernelUpdater updater)
        {
            _updater = updater;

            EditorApplication.update += OnTick;
            SceneView.beforeSceneGui += OnGUI;
            AssemblyReloadEvents.beforeAssemblyReload += Clear;
        }


        /// <summary>
        /// Clears the subscriptions to update hooks.
        /// </summary>
        public void Clear()
        {
            _disposed = true;
            EditorApplication.update -= OnTick;
            SceneView.beforeSceneGui -= OnGUI;
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

            _updater.Input(Event.current);
            _updater.Draw(true);
            sceneView.Repaint();
        }
    }
}
#endif
