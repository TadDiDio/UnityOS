#if UNITY_EDITOR
using UnityEditor;
#endif
using Codice.Client.BaseCommands;
using UnityEngine;

namespace DeveloperConsole
{
    public class KernelUpdater
    {
        public void Tick()
        {
            if (!Kernel.IsInitialized)
            {
                Debug.LogWarning("Kernel not initialized, yet edit mode ticker is. This should not happen. Skipping tick.");
                return;
            }
            
#if UNITY_EDITOR
            // Need to call this to repaint every frame rather than waiting for an event to update async output
            SceneView.RepaintAll();
#endif
            Kernel.Instance.Tick();
        }

        public void Input(Event current)
        {
            // TODO: May need to verify that only a single unified input stream passes through here.
            
            // TODO: Allow other event types through as needed
            if (current.type is not EventType.KeyDown) return;
            
            if (!Kernel.IsInitialized)
            {
                Debug.LogWarning("Kernel not initialized, yet edit mode ticker is. This should not happen. Skipping OnGUI.");
                return;
            }
            
            Kernel.Instance.OnInput(current);
        }
        public void Draw(int screenWidth, int screenHeight, bool sceneView)
        {
            if (!Kernel.IsInitialized)
            {
                Debug.LogWarning("Kernel not initialized, yet edit mode ticker is. This should not happen. Skipping OnGUI.");
                return;
            }
            
            Handles.BeginGUI();
            Kernel.Instance.OnDraw(screenWidth, screenHeight, sceneView);
            Handles.EndGUI();
        }
    }
}