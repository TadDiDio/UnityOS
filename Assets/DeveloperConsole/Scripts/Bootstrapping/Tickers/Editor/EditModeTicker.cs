using UnityEditor;

namespace DeveloperConsole
{
    /// <summary>
    /// Captures logical and graphical update events to forward to the kernel while in edit mode.
    /// Acts as the edit mode entry point for the system.
    /// </summary>
    public static class EditModeTicker
    {
        private const int ToolbarHeight = 25;
        
        public static void Initialize()
        {
            StaticResetRegistry.Register(Reset);
            EditorApplication.update += OnTick;
            SceneView.duringSceneGui += OnGUI;
            AssemblyReloadEvents.beforeAssemblyReload += Reset;
        }


        private static void Reset()
        {
            EditorApplication.update -= OnTick;
            SceneView.duringSceneGui -= OnGUI;
        }
        
        private static void OnTick() => ConsoleKernel.Tick();
        private static void OnGUI(SceneView sceneView)
        {
            int width = (int)sceneView.position.width;
            int height = (int)sceneView.position.height - ToolbarHeight;
            ConsoleKernel.OnGUI(width, height);
        }
    }
}