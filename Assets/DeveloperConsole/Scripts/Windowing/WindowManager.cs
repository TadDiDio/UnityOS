using System.Collections.Generic;
using UnityEngine;

namespace DeveloperConsole.Windowing
{
    // TODO: This class will need major refactoring. For now it just spawns a terminal.
    // I think eventually the / toggle should only affect the terminal, while all other windows are persistent
    // until explicitly minimized or closed.
    /// <summary>
    /// Manages and updates windows.
    /// </summary>
    public class WindowManager : IWindowManager
    {
        private List<IWindow> _windows = new();

        private bool _visible;

        public void RegisterWindow(IWindow window)
        {
            if (_windows.Contains(window)) return;
            _windows.Add(window);
        }


        public void UnregisterWindow(IWindow window)
        {
            if (!_windows.Contains(window)) return;
            _windows.Remove(window);
        }


        public void OnInput(Event current)
        {
            // Must use characters here because unity generates two keypress events for each
            // physical press, one is a physical key and the other textual. If you only
            // handle physical keys (KeyCode), then TextFields can still receive and consume textual presses.
            if (Event.current.type is EventType.KeyDown && Event.current.character == '/')
            {
                _visible = !_visible;
                Event.current.Use();
                return;
            }

            if (!_visible) return;
            foreach (var window in _windows) window.OnInput(Event.current);
        }

        public void OnGUI(Rect fullScreen)
        {
            if (!_visible) return;

            fullScreen = new Rect(fullScreen.x, fullScreen.y, fullScreen.width * 0.9f, fullScreen.height);
            foreach (var window in _windows) window.Draw(fullScreen);
        }
    }
}
