using UnityEngine;
using UnityEngine.UIElements;

namespace DeveloperConsole.Windowing
{
    public class DraggableBehavior : IWindowBehavior
    {
        private bool _dragging;
        private Vector2 _dragStart;
        private Vector2 _startPosition;

        public void Attach(WindowModel model, VisualElement root)
        {
            root.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.button != 0) return; // only left mouse
                _dragging = true;
                _dragStart = evt.mousePosition;
                _startPosition = new Vector2(root.resolvedStyle.left, root.resolvedStyle.top);
                root.CaptureMouse();
                evt.StopPropagation();
            });

            root.RegisterCallback<MouseMoveEvent>(evt =>
            {
                if (!_dragging) return;

                Vector2 delta = evt.mousePosition - _dragStart;
                float newLeft = _startPosition.x + delta.x;
                float newTop = _startPosition.y + delta.y;

                root.style.left = newLeft;
                root.style.top = newTop;

                evt.StopPropagation();
            });

            root.RegisterCallback<MouseUpEvent>(evt =>
            {
                if (!_dragging) return;
                _dragging = false;
                root.ReleaseMouse();
                evt.StopPropagation();
            });

            // Optional: cancel drag if mouse leaves window
            root.RegisterCallback<MouseLeaveEvent>(_ =>
            {
                if (_dragging)
                {
                    _dragging = false;
                    root.ReleaseMouse();
                }
            });
        }
    }
}
