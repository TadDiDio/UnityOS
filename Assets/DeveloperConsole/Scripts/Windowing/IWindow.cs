using UnityEngine;

namespace DeveloperConsole
{
    public interface IWindow
    {
        public void Draw(Rect areaRect);
        public void OnInput(Event current);

        public void OnShow();
        public void OnHide();
    }
}