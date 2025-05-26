using UnityEngine;

namespace DeveloperConsole
{
    public interface IGraphical
    {
        public void OnGUI(GUIContext context);
    }

    public struct GUIContext
    {
        public Rect AreaRect;
        public GUIStyle Style;
    }
}