using System.Collections.Generic;
using UnityEngine;

namespace DeveloperConsole
{
    public class WindowManager : IWindowManager
    {
        private List<IWindow> _graphicalComponents = new();
        
        public void RegisterWindow(IWindow window)
        {
            if (_graphicalComponents.Contains(window)) return;
            _graphicalComponents.Add(window);
        }
        
        public void UnregisterWindow(IWindow window)
        {
            if (!_graphicalComponents.Contains(window)) return;
            _graphicalComponents.Remove(window);
        }

        public void OnGUI(Rect fullScreen)
        {
            foreach (var graphical in _graphicalComponents) graphical.OnGUI(fullScreen);
        }
    }
}