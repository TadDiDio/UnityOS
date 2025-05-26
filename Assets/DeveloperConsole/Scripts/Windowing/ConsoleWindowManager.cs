using System.Collections.Generic;

namespace DeveloperConsole
{
    public static class ConsoleWindowManager
    {
        private static readonly List<ConsoleWindow> _windows = new();
        
        public static void RegisterWindow(ConsoleWindow window)
        {
            if (_windows.Contains(window))
            {
                // TODO: Console error
                return;
            }
            
            _windows.Add(window);
        }

        public static void UnregisterWindow(ConsoleWindow window)
        {
            if (!_windows.Contains(window))
            {
                // TODO: Console error
                return;
            }
            
            _windows.Remove(window);
        }
        
        public static void Tick()
        {
            foreach (var window in _windows) window.Tick();
        }
    }
}