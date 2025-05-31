using UnityEngine;

namespace DeveloperConsole
{
    public interface IWindowManager
    {
        public void RegisterWindow(IWindow window);
        public void UnregisterWindow(IWindow window);
        public void OnGUI(Rect fullScreen);
    }
}