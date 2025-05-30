using System.Collections.Generic;

namespace DeveloperConsole
{
    public class WindowManager : IWindowManager
    {
        private List<IGraphical> _graphicalComponents = new();
        
        public void Register(IGraphical graphical)
        {
            if (_graphicalComponents.Contains(graphical))
            {
                // TODO: Log error
                return;
            }
            
            _graphicalComponents.Add(graphical);
        }
        
        public void Unregister(IGraphical graphical)
        {
            if (!_graphicalComponents.Contains(graphical))
            {
                // TODO: Log error
                return;
            }
            
            _graphicalComponents.Remove(graphical);
        }

        public void OnGUI(GUIContext context)
        {
            foreach (var graphical in _graphicalComponents) graphical.OnGUI(context);
        }
    }
}