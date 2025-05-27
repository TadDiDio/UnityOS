using UnityEngine;

namespace DeveloperConsole
{
    public abstract class ConsoleWindow : IGraphical, ITickable
    {
        public abstract string Title { get; }
        public abstract Rect WindowRect { get; set; }

        protected ConsoleWindow() => ConsoleKernel.RegisterTickable(this);
        ~ConsoleWindow() => ConsoleKernel.UnregisterTickable(this);
        
        public abstract void OnGUI(GUIContext context);
        public abstract void Tick();
    }
}