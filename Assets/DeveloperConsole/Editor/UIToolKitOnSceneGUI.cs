using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[InitializeOnLoad]
public class UIToolkitIntegration
{
    static UIToolkitIntegration()
    {
        SceneView.duringSceneGui += OnScene;
    }

    private static void OnScene(SceneView sceneView)
    {
        if (sceneView.rootVisualElement.Q("my-scene-ui") == null)
        {
            var container = new VisualElement { name = "my-scene-ui" };

            var button = new Button(() => Debug.Log("Scene button clicked"))
            {
                text = "Hello SceneView"
            };

            container.Add(button);
            sceneView.rootVisualElement.Add(container);
        }
    }
}
