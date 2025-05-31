using UnityEngine;

namespace DeveloperConsole
{
    /// <summary>
    /// Forwards logical and gui update events to the kernel while in play mode.
    /// </summary>
    public class PlayModeTicker : MonoBehaviour
    {
        // TODO: Need to handle duplicate playmode tickers here

        private void Update()
        {
            if (!Kernel.IsInitialized)
            {
                Debug.LogWarning("Kernel not initialized, yet play mode ticker is. This should not happen. Skipping tick.");
                return;
            }
            
            Kernel.Instance.Tick();
        }
        private void OnGUI()
        { 
            if (!Kernel.IsInitialized)
            {
                Debug.LogWarning("Kernel not initialized, yet play mode ticker is. This should not happen. Skipping OnGUI.");
                return;
            }
            
            Kernel.Instance.OnGUI(Screen.width, Screen.height);
        }
    }
}