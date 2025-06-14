using UnityEngine;

namespace DeveloperConsole.Windowing
{
    /// <summary>
    /// A drawable window.
    /// </summary>
    public interface IWindow
    {
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

        
        /// <summary>
        /// Called when this window shows.
        /// </summary>
        public void OnShow();
        
        
        /// <summary>
        /// Called when this window hides.
        /// </summary>
        public void OnHide();
    }
}