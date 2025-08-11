using System;
using UnityEngine;

namespace DeveloperConsole.Windowing
{
    public abstract class WindowBase : IWindow
    {
        private static int _nextWindowId;
        protected int WindowId;
        private Rect _windowRect;

        private WindowConfig _config;

        public event Action<IWindow> OnClose;

        public Action<IWindow> OnHide;

        protected WindowBase(WindowConfig config)
        {
            WindowId = _nextWindowId++;
            _config = config;
            _windowRect = new Rect(0, 0, _config.MinSize.x, _config.MinSize.y);
        }

        public WindowConfig GetConfig() => _config;

        public void SetName(string name)
        {
            _config.Name = name;
        }

        public void Draw(Rect fullScreen)
        {
            fullScreen = new Rect(fullScreen.x + _config.Padding, fullScreen.y + _config.Padding,
                fullScreen.width - 2 * _config.Padding, fullScreen.height - 2 * _config.Padding);

            if (_config.FullScreen) _windowRect = fullScreen;
            _windowRect = GUILayout.Window(WindowId, _windowRect, DrawWindow, _config.Name);

            // Clamp to screen limits
            _windowRect.x = Mathf.Clamp(_windowRect.x, 0, fullScreen.width - _windowRect.width);
            _windowRect.y = Mathf.Clamp(_windowRect.y, 0, fullScreen.height - _windowRect.height);
        }

        private void DrawWindow(int id)
        {
            DrawHeader();
            DrawBody();
            if (_config.IsResizeable) HandleResize();
            if (_config.IsDraggable) GUI.DragWindow();
        }

        private void DrawHeader()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (_config.IsMinimizable)
            {
                if (GUILayout.Button("-")) OnHide?.Invoke(this);
            }
            if (_config.Closeable)
            {
                if (GUILayout.Button("x")) OnClose?.Invoke(this);
            }

            GUILayout.EndHorizontal();
        }

        private void DrawBody()
        {
            GUILayout.Space(_config.HeaderHeight);
            DrawContent(_windowRect);
        }


        private bool _resizing;
        private Vector2 _resizeStartMousePos;
        private Rect _resizeStartWindowRect;
        private const float ResizeHandleSize = 16f;
        private void HandleResize()
        {
            Rect resizeHandleRect = new Rect(_windowRect.width - ResizeHandleSize, _windowRect.height - ResizeHandleSize, ResizeHandleSize, ResizeHandleSize);

            // Handle mouse events for resizing
            Event e = Event.current;
            if (e.type == EventType.MouseDown && resizeHandleRect.Contains(e.mousePosition))
            {
                _resizing = true;
                _resizeStartMousePos = e.mousePosition;
                _resizeStartWindowRect = _windowRect;
                e.Use();
            }
            else if (e.type == EventType.MouseDrag && _resizing)
            {
                Vector2 delta = e.mousePosition - _resizeStartMousePos;
                _windowRect.width = Mathf.Max(_config.MinSize.x, _resizeStartWindowRect.width + delta.x);
                _windowRect.height = Mathf.Max(_config.MinSize.y, _resizeStartWindowRect.height + delta.y);
                e.Use();
            }
            else if (e.type == EventType.MouseUp && _resizing)
            {
                _resizing = false;
                e.Use();
            }
        }

        protected abstract void DrawContent(Rect areaRect);

        public abstract void OnInput(Event current);
    }
}
