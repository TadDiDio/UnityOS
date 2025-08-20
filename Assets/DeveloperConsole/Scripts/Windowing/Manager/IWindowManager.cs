using System.Collections.Generic;

namespace DeveloperConsole.Windowing
{
    /// <summary>
    /// Manages and updates windows.
    /// </summary>
    public interface IWindowManager
    {
        public void AddWindow(WindowModel model, IEnumerable<IWindowBehavior> behaviors);

        /// <summary>
        /// Removes a window.
        /// </summary>
        /// <param name="window">The window.</param>
        public void RemoveWindow(WindowController window);
    }
}
