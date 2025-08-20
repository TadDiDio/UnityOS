#if UNITY_EDITOR
using UnityEditor;
#endif

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
    }
}
