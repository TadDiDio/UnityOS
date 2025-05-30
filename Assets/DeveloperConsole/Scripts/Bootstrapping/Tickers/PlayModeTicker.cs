using UnityEngine;

namespace DeveloperConsole
{
    /// <summary>
    /// Forwards logical and gui update events to the kernel while in play mode.
    /// </summary>
    public class PlayModeTicker : MonoBehaviour
    {
        // TODO: May need to handle duplicates here since bootstrapper checks before scene load

        private void Update() => Kernel.Instance.Tick();
        private void OnGUI() => Kernel.Instance.OnGUI(Screen.width, Screen.height);
    }
}