using System.Collections.Generic;
using DeveloperConsole.Core;
using UnityEngine;

namespace DeveloperConsole.Windowing
{
    // TODO: This class will need major refactoring. For now it just spawns a terminal.
    /// <summary>
    /// Manages and updates windows.
    /// </summary>
    public class WindowManager : IWindowManager
    {
        private List<IWindow> _windows = new();
        
        private bool _visible;
        private bool _wasVisible;

        private IShellApplication _shell;
        
        public WindowManager(IShellApplication shell)
        {
            _shell = shell;

            var terminal = new TerminalApplication(shell.CreateSession());
            _shell.InputManager.RegisterInputSource(terminal);
            _shell.OutputManager.RegisterOutputSink(terminal);
            
            RegisterWindow(terminal);
        }
        
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
            // handle physical keys (KeyCode), then TextFields can still consume textual presses.
            if (Event.current.type is EventType.KeyDown && Event.current.character == '/')
            {
                _visible = !_visible;
                Event.current.Use();
                return;
            }

            foreach (var window in _windows) window.OnInput(Event.current);
        }
        
        
        public void OnGUI(Rect fullScreen)
        {
            if (_visible != _wasVisible)
            {
                if (_visible) foreach (var window in _windows) window.OnShow();
                else foreach (var window in _windows) window.OnHide();
            }
            
            _wasVisible = _visible;
            if (!_visible) return;
            
            fullScreen = new Rect(fullScreen.x, fullScreen.y, fullScreen.width * 0.9f, fullScreen.height);
            foreach (var window in _windows) window.Draw(fullScreen);
        }
    }
}