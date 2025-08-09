using UnityEngine;

namespace DeveloperConsole.Windowing
{
    public class WindowConfig
    {
        public bool ForceFullscreen;
        public bool Closeable = false;
        public bool IsMinimizable = false;
        public bool IsDraggable = false;
        public bool IsResizeable = false;
        public Vector2 MinSize = new(100, 50);
        public Vector2 MaxSize = new(0, 0); // (0, 0) limits to screen size

        public string Name = "Window";
        public int Padding = 5;
        public int HeaderHeight = 20;
    }
}
