using System.Collections.Generic;

namespace DeveloperConsole
{
    public static class GraphicsManager
    {
        private static List<IGraphical> _graphicalComponents = new();

        public static void Initialize()
        {
            StaticResetRegistry.Register(UnregisterAll);
        }
        
        public static void Register(IGraphical graphical)
        {
            if (_graphicalComponents.Contains(graphical))
            {
                // TODO: Log error
                return;
            }
            
            _graphicalComponents.Add(graphical);
        }
        
        public static void Unregister(IGraphical graphical)
        {
            if (!_graphicalComponents.Contains(graphical))
            {
                // TODO: Log error
                return;
            }
            
            _graphicalComponents.Remove(graphical);
        }

        public static void UnregisterAll() => _graphicalComponents.Clear();
        
        public static void OnGUI(GUIContext context)
        {
            foreach (var graphical in _graphicalComponents) graphical.OnGUI(context);
        }
    }
}