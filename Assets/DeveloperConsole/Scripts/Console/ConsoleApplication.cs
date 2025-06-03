using System;
using UnityEngine;

namespace DeveloperConsole
{
    public class ConsoleApplication : ShellApplication, IWindow, IInputSource, IOutputSink
    {
        public event Action<string> InputSubmitted;
        
        private ConsoleState _consoleState;

        private int _historyIndexFromEnd = 0;
        private string _inputBuffer = "";
        private Vector2 _scrollPosition = Vector2.zero;
        
        public ConsoleApplication(ITokenizationManager tokenizationManager, ICommandParser parser) 
            : base(tokenizationManager, parser)
        {
            // TODO: Move this or inject it or a factory.
            _consoleState = JsonFileManager.Load();

            InputManager.RegisterInputSource(this);
            OutputManager.RegisterOutputSink(this);
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
            foreach (string line in _consoleState.OutputBuffer)
            {
                GUILayout.Label(line, ConsoleGUIStyle.DefaultStyle());
            }

            // Input field inline with output
            GUILayout.BeginHorizontal();
            
            GUILayout.Label(GetPrompt(), ConsoleGUIStyle.Prompt(), GUILayout.ExpandWidth(false));
            GUI.SetNextControlName("TerminalInputField");
            _inputBuffer = GUILayout.TextField(_inputBuffer, ConsoleGUIStyle.InputField());
            
            //if (_needsFocus) 
            GUI.FocusControl("TerminalInputField");
            
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        public void OnInput(Event current)
        {
            if (current.type is not EventType.KeyDown) return;
            
            _scrollPosition.y = float.MaxValue;

            if (current.keyCode == KeyCode.UpArrow)
            {
                _historyIndexFromEnd = Mathf.Min(_consoleState.CommandHistory.Count, _historyIndexFromEnd + 1);
                if (_consoleState.TryGetHistory(_historyIndexFromEnd, out string history))
                {
                    _inputBuffer = history;
                }
                current.Use();
            }
            else if (current.keyCode == KeyCode.DownArrow)
            {
                if (_historyIndexFromEnd == 0) return;
                
                _historyIndexFromEnd = Mathf.Max(1, _historyIndexFromEnd - 1);
                if (_consoleState.TryGetHistory(_historyIndexFromEnd, out string history))
                {
                    _inputBuffer = history;
                }
                current.Use();
            }
            else if (current.keyCode is KeyCode.Return or KeyCode.KeypadEnter) 
            {
                InputSubmitted?.Invoke(_inputBuffer);
                _consoleState.AddCommandHistory(_inputBuffer);
                _inputBuffer = "";
                _historyIndexFromEnd = 0;
                current.Use();
            }
        }

        public void OnShow() { }

        public void OnHide()
        {
            _inputBuffer = "";
        }

        protected override CommandContext GetSpecificCommandContext()
        {
            return new CommandContext
            {
                ConsoleState = _consoleState
            };
        }

        protected override void OnAfterInputProcessed(CommandRunResult result)
        {
            // TODO: Update to support file per console
            JsonFileManager.Save(_consoleState);
        }
        
        public void ReceiveOutput(string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            _consoleState.AddOutput(message);
        }
    }
}