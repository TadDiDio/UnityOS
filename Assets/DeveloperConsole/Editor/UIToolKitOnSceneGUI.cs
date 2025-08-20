using DeveloperConsole;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.UIElements;

[InitializeOnLoad]
public class UIToolkitIntegration
{
    static UIToolkitIntegration()
    {
        // SceneView.duringSceneGui += OnScene;
    }

    private static void OnScene(SceneView sceneView)
    {
        if (sceneView.rootVisualElement.Q("my-scene-ui") != null)
            return;

        var container = new VisualElement { name = "my-scene-ui" };

        // === make it fullscreen ===
        container.style.position = Position.Absolute;
        container.style.left = 5;
        container.style.top = 5;
        container.style.right = 5;
        container.style.bottom = 5;
        container.style.backgroundColor = new Color(0, 0, 0, 0.4f); // translucent overlay
        container.style.flexDirection = FlexDirection.Column;
        container.style.paddingLeft = 8;
        container.style.paddingTop = 8;

        // --- TextField test
        var textField = new TextField();

        textField.RegisterCallback<NavigationSubmitEvent>(evt => {
            textField.value = "";
            evt.StopPropagation();
            textField.Focus();
        }, TrickleDown.TrickleDown);

        container.Add(textField);

        // --- Global key capture
        container.focusable = true;
        container.RegisterCallback<MouseDownEvent>(evt =>
        {
            textField.Focus();
        });

        sceneView.rootVisualElement.Add(container);

        // optional: auto-focus so it captures keys right away
        container.Focus();
    }
}
