using UnityEngine;

namespace DeveloperConsole.Windowing
{
    /// <summary>
    /// Manages and updates windows.
    /// </summary>
    public interface IWindowManager
    {
        /// <summary>
        /// Registers a new window to manager.
        /// </summary>
        /// <param name="window">The window.</param>
        public void RegisterWindow(IWindow window);

        /// <summary>
        /// Unregisters a window.
        /// </summary>
        /// <param name="window">The window.</param>
        public void UnregisterWindow(IWindow window);

        /// <summary>
        /// Distributes input to the correct window(s).
        /// </summary>
        /// <param name="current">The current input.</param>
        public void OnInput(Event current);

        /// <summary>
        /// Draws all windows based on the full screen available.
        /// </summary>
        /// <param name="isSceneView">True if this call is in the scene view.</param>
        public void OnGUI(bool isSceneView);
    }
}
