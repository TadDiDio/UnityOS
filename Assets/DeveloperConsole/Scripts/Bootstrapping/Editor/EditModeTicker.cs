using UnityEditor;

namespace DeveloperConsole
{
    /// <summary>
    /// Captures logical and graphical update events to forward to the kernel while in edit mode.
    /// Acts as the edit mode entry point for the system.
    /// </summary>
    [InitializeOnLoad]
    public static class EditModeTicker
    {
        private const int ToolbarHeight = 25;
        
        static EditModeTicker()
        {
            EditorApplication.update += OnTick;
            SceneView.duringSceneGui += OnGui;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
        }

        private static void OnBeforeAssemblyReload()
        {
            EditorApplication.update -= OnTick;
            SceneView.duringSceneGui -= OnGui;
        }
        
        private static void OnTick() => ConsoleKernel.Tick();

        private static void OnGui(SceneView sceneView)
        {
            int width = (int)sceneView.position.width;
            int height = (int)sceneView.position.height - ToolbarHeight;
            ConsoleKernel.OnGUI(width, height);
        }
    }
}