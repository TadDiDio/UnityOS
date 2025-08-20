using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DeveloperConsole.Windowing
{
    public class WindowController
    {
        public VisualElement Root;
        public WindowModel Model;
        private IEnumerable<IWindowBehavior> _behaviors;

        public const string RootName    = "Root";
        public const string HeaderName  = "Header";
        public const string ContentName = "Content";


        public WindowController(WindowModel model, IEnumerable<IWindowBehavior> behaviors)
        {
            Model = model;
            _behaviors = behaviors;

            var stylesheet = Resources.Load<StyleSheet>("WindowStyle");
            Root = new VisualElement { name = RootName };
            Root.styleSheets.Add(stylesheet);

            var header = new VisualElement { name = HeaderName };
            var titleLabel = new Label("New Window") { name = "HeaderTitle" };
            var buttonContainer = new VisualElement { name = "HeaderButtons" };
            var content = new VisualElement { name = ContentName };

            header.Add(titleLabel);
            header.Add(buttonContainer);

            Root.Add(header);
            Root.Add(content);

            foreach (var behavior in _behaviors)
            {
                behavior.Attach(Model, Root);
            }
        }
    }
}
