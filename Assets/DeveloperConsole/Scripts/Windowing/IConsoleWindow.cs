using UnityEngine;

namespace DeveloperConsole
{
    public abstract class ConsoleWindow : IGraphical, ITickable
    {
        public abstract string Title { get; }
        public abstract Rect WindowRect { get; set; }

        public ConsoleWindow() => ConsoleKernel.Register(this);
        ~ConsoleWindow() => ConsoleKernel.Unregister(this);
        
        public abstract void OnGUI(GUIContext context);
        public abstract void Tick();
    }
}