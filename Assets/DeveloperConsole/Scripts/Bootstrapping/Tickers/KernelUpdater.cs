#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using DeveloperConsole.Core.Kernel;

namespace DeveloperConsole
{

    /// <summary>
    /// Manages streaming tick, GUI, and input events to the kernel.
    /// </summary>
    public class KernelUpdater
    {
        private bool _disabled;

        /// <summary>
        /// Ticks the kernel.
        /// </summary>
        public void Tick()
        {
            if (_disabled) return;

            if (!Kernel.IsInitialized)
            {
                Log.Error("Kernel not initialized, yet edit mode ticker is. This should not happen. Console will not initialize.");
                _disabled = true;
                return;
            }

#if UNITY_EDITOR
            // Need to call this to repaint every frame rather than waiting for an event to update async output
            SceneView.RepaintAll();
#endif
            Kernel.Instance.Tick();
        }


        /// <summary>
        /// Ticks the kernal input hook.
        /// </summary>
        /// <param name="current">The current event.</param>
        public void Input(Event current)
        {
            if (_disabled) return;

            // TODO: May need to verify that only a single unified input stream passes through here.

            // TODO: Allow other event types through as needed
            if (current.type is not EventType.KeyDown) return;

            if (!Kernel.IsInitialized)
            {
                Log.Error("Kernel not initialized, yet edit mode ticker is. This should not happen. Console will not initialize.");
                _disabled = true;
                return;
            }

            Kernel.Instance.OnInput(current);
        }


        /// <summary>
        /// Ticks the kernel draw hook.
        /// </summary>
        /// <param name="screenWidth">The screen width.</param>
        /// <param name="screenHeight">The screen height.</param>
        /// <param name="sceneView">The SceneView.</param>
        public void Draw(int screenWidth, int screenHeight, bool sceneView)
        {
            if (_disabled) return;

            if (!Kernel.IsInitialized)
            {
                Log.Error("Kernel not initialized, yet edit mode ticker is. This should not happen. Console will not initialize.");
                _disabled = true;
                return;
            }

#if UNITY_EDITOR
            Handles.BeginGUI();
#endif
            Kernel.Instance.OnDraw(screenWidth, screenHeight, sceneView);

#if UNITY_EDITOR
            Handles.EndGUI();
#endif
        }
    }
}
