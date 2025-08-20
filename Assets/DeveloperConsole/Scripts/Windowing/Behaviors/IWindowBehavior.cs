using UnityEngine.UIElements;

namespace DeveloperConsole.Windowing
{
    public interface IWindowBehavior
    {
        public void Attach(WindowModel model, VisualElement root);
    }
}
