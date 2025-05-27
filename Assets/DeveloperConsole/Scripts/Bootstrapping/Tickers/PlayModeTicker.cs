using UnityEngine;

namespace DeveloperConsole
{
    /// <summary>
    /// Forwards logical and gui update events to the kernel while in play mode.
    /// </summary>
    public class PlayModeTicker : MonoBehaviour
    {
        // TODO: May need to handle duplicates here since bootstrapper checks before scene load

        private void Update() => ConsoleKernel.Tick();
        private void OnGUI() => ConsoleKernel.OnGUI(Screen.width, Screen.height);
    }
}