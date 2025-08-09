using System;
using UnityEngine;

namespace DeveloperConsole.Windowing
{
    /// <summary>
    /// A drawable window.
    /// </summary>
    public interface IWindow
    {
        /// <summary>
        /// Invoked when the window is closed by the user.
        /// </summary>
        public event Action<IWindow> OnClose;


        /// <summary>
        /// Sets the name of this window.
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name);

        /// <summary>
        /// Draws the window.
        /// </summary>
        /// <param name="areaRect">The area that this window gets to fill.</param>
        public void Draw(Rect areaRect);


        /// <summary>
        /// Handles input.
        /// </summary>
        /// <param name="current">The input.</param>
        public void OnInput(Event current);
    }
}
