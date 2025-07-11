using System;
using DeveloperConsole.Core.Shell;
using UnityEngine;
using DeveloperConsole.IO;
using DeveloperConsole.Windowing;

namespace DeveloperConsole
{
    public class TerminalApplication : IWindow, IShellClient
    {
        public Guid ShellSessionId { get; }
        public event Action<IInput> InputSubmitted;

        private ShellSession _session;
        
        private bool _shown;
        private int _historyIndexFromEnd;
        private string _inputBuffer = "";
        private Vector2 _scrollPosition = Vector2.zero;


        public TerminalApplication(ShellSession session)
        {
            _session = session;
        }
        
        public void Draw(Rect areaRect)
        {
            // Terminal background
            var previousColor = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 1f);
            GUI.Box(areaRect, GUIContent.none);
            GUI.color = previousColor;

            GUILayout.BeginArea(areaRect);
            GUILayout.BeginVertical();
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));

            // Print each output line
            foreach (string line in _session.OutputBuffer)
            {
                GUILayout.Label(line, TerminalGUIStyle.DefaultStyle());
            }

            // Input field inline with output
            GUILayout.BeginHorizontal();
            
            GUILayout.Label("> ", TerminalGUIStyle.Prompt(), GUILayout.ExpandWidth(false));
            GUI.SetNextControlName("TerminalInputField");
            _inputBuffer = GUILayout.TextField(_inputBuffer, TerminalGUIStyle.InputField());
            
            GUI.FocusControl("TerminalInputField");
            
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        public void OnInput(Event current)
        {
            if (!_shown) return;
            if (current.type is not EventType.KeyDown) return;
            
            _scrollPosition.y = float.MaxValue;

            if (current.keyCode == KeyCode.UpArrow)
            {
                _historyIndexFromEnd = Mathf.Min(_session.CommandHistory.Count, _historyIndexFromEnd + 1);
                if (_session.TryGetHistory(_historyIndexFromEnd, out string history))
                {
                    _inputBuffer = history;
                }
                current.Use();
            }
            else if (current.keyCode == KeyCode.DownArrow)
            {
                if (_historyIndexFromEnd == 0) return;
                
                _historyIndexFromEnd = Mathf.Max(1, _historyIndexFromEnd - 1);
                if (_session.TryGetHistory(_historyIndexFromEnd, out string history))
                {
                    _inputBuffer = history;
                }
                current.Use();
            }
            else if (current.keyCode is KeyCode.Return or KeyCode.KeypadEnter) 
            {
                InputSubmitted?.Invoke(new TextInput(_session, _inputBuffer));
                if (!string.IsNullOrEmpty(_inputBuffer)) _session.AddCommandHistory(_inputBuffer);
                _inputBuffer = "";
                _historyIndexFromEnd = 0;
                current.Use();
            }
        }

        public void OnShow()=> _shown = true;

        public void OnHide() => _shown = false;

        public void ReceiveOutput(IOutputMessage message)
        {
            _session.AddOutput(message.Message());
        }

        public void Prompt(PromptContext context)
        {
            
        }
    }
}