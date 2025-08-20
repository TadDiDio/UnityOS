using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace DeveloperConsole.Windowing
{
    /// <summary>
    /// Manages and updates windows.
    /// </summary>
    public class WindowManager : IWindowManager
    {
        private bool _pendingAdd;
        private UIDocument _uiDocument;
        private WindowController _sceneController;

        /// <summary>
        /// Creates a new WindowManager.
        /// </summary>
        /// <param name="uiDocument">The playmode UI document to bind UI to. Is null outside of runtime.</param>
        public WindowManager(UIDocument uiDocument)
        {
            _uiDocument = uiDocument;
            SceneView.duringSceneGui += OnSceneGui;
        }

        private void OnSceneGui(SceneView sceneView)
        {
            if (!_pendingAdd) return;

            sceneView.rootVisualElement.Add(_sceneController.Root);

            _pendingAdd = false;
        }

        public void AddWindow(WindowModel model, IEnumerable<IWindowBehavior> behaviors)
        {
            _sceneController = new WindowController(model, behaviors);
            var gameController = new WindowController(model, behaviors);

            if (_uiDocument)
            {
                _uiDocument.rootVisualElement.Add(gameController.Root);
            }
            _pendingAdd = true;
        }

        public void RemoveWindow(WindowController window)
        {
            throw new System.NotImplementedException();
        }
    }
}
