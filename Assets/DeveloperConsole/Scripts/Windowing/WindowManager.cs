using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DeveloperConsole.Windowing
{
    // TODO: This class will need major refactoring. For now it just spawns a terminal.
    // I think eventually the / toggle should only affect the terminal, while all other windows are persistent
    // until explicitly minimized or closed.
    /// <summary>
    /// Manages and updates windows.
    /// </summary>
    public class WindowManager : IWindowManager
    {
        private List<IWindow> _windows = new();

        private TerminalClient _terminal;
        private bool _terminalVisible;

        private Rect _screenRect;

        private GUISkin _gammaSkin = Resources.Load<GUISkin>("Gamma_Skin");
        private GUISkin _linearSkin = Resources.Load<GUISkin>("Linear_Skin");

        private const int ToolbarHeight = 25;

        public void RegisterWindow(IWindow window)
        {
            // TODO: This is temporary. Nothing should be special about terminal
            if (window is TerminalClient terminal)
            {
                _terminal = terminal;
                return;
            }

            if (_windows.Contains(window)) return;

            window.OnClose += OnWindowClose;

            _windows.Add(window);
        }

        public void UnregisterWindow(IWindow window)
        {
            if (!_windows.Contains(window)) return;

            window.OnClose -= OnWindowClose;

            _windows.Remove(window);
        }


        private void OnWindowClose(IWindow window)
        {
            UnregisterWindow(window);
        }

        public void OnInput(Event current)
        {
            if (current.character == '/')
            {
                _terminalVisible = !_terminalVisible;

                if (_terminalVisible) _terminal.Focus();
                current.Use();
                return;
            }


            if (_terminalVisible) _terminal.OnInput(current);
            foreach (var window in _windows.ToList()) window.OnInput(current);
        }

        public void OnGUI(bool isSceneView)
        {
            bool useGamma = isSceneView || QualitySettings.activeColorSpace is ColorSpace.Gamma;
            GUI.skin = useGamma ? _gammaSkin : _linearSkin;

            Rect fullScreen = GetFullScreen(isSceneView);

            foreach (var window in _windows.ToList()) window.Draw(fullScreen);

            if (_terminalVisible) _terminal.Draw(fullScreen);
        }

        private Rect GetFullScreen(bool isSceneView)
        {
#if UNITY_EDITOR
            if (isSceneView)
            {
                int width = (int)SceneView.currentDrawingSceneView.position.width;
                int height = (int)SceneView.currentDrawingSceneView.position.height - ToolbarHeight;
                return new Rect(0, 0, width, height);
            }
            else
            {
                int width = Screen.width;
                int height = Screen.height;
                return new Rect(0, 0, width, height);
            }
#else
            return new Rect(0, 0, Screen.width, Screen.height);
#endif
        }
    }
}
