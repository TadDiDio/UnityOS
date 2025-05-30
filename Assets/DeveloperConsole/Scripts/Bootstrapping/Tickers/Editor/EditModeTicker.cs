using UnityEditor;

namespace DeveloperConsole
{
    /// <summary>
    /// Captures logical and graphical update events to forward to the kernel while in edit mode.
    /// Acts as the edit mode entry point for the system.
    /// </summary>
    public class EditModeTicker : Singleton<EditModeTicker>
    {
        private const int ToolbarHeight = 25;
        
        public EditModeTicker()
        {
            EditorApplication.update += OnTick;
            SceneView.duringSceneGui += OnGUI;
            AssemblyReloadEvents.beforeAssemblyReload += Reload;
        }

        private void Reload()
        {
            EditorApplication.update -= OnTick;
            SceneView.duringSceneGui -= OnGUI;
        }
        
        private void OnTick() => Kernel.Instance.Tick();
        private void OnGUI(SceneView sceneView)
        {
            int width = (int)sceneView.position.width;
            int height = (int)sceneView.position.height - ToolbarHeight;
            Kernel.Instance.OnGUI(width, height);
        }
    }
}