using System;
using UnityEngine;

namespace DeveloperConsole
{
    public class ConsoleApplication : ShellApplication, IWindow, IInputSource, IOutputSink
    {
        public event Action<string> InputSubmitted;
        
        private ConsoleState _consoleState;
        
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
        
        public void OnGUI(Rect areaRect)
        {
            try
            {
                // Terminal background
                var previousColor = GUI.color;
                GUI.color = new Color(0f, 0f, 0f, 1f); 
                GUI.Box(areaRect, GUIContent.none); 
                GUI.color = previousColor;
                
                GUILayout.BeginArea(areaRect);
                GUILayout.BeginVertical();
                
                // Scrollable output + input area
                // TODO Handle overflow
                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));
                
                // Print each output line
                foreach (string line in _consoleState.OutputBuffer)
                {
                    GUILayout.Label(line, ConsoleGUIStyle.DefaultStyle());
                }
                
                // Input field inline with output
                GUILayout.BeginHorizontal();
                // TODO: This is not where prompt is decided
                GUILayout.Label("> ", ConsoleGUIStyle.Prompt(), GUILayout.ExpandWidth(false));
                GUI.SetNextControlName("TerminalInputField");
                _inputBuffer = GUILayout.TextField(_inputBuffer, ConsoleGUIStyle.InputField());
                GUILayout.EndHorizontal();
                
                GUI.FocusControl("TerminalInputField");
                
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                
                // Submit on Enter (only when input has focus)
                var e = Event.current;
                
                if (e.type is EventType.KeyDown) _scrollPosition.y = float.MaxValue;
                if (e.type is EventType.KeyDown && e.character is '\n' or '\r') 
                {
                    InputSubmitted?.Invoke(_inputBuffer);
                    _inputBuffer = "";
                    e.Use();
                }
            }
            finally
            {
                GUILayout.EndArea();
            }
        }

        protected override CommandContext GetSpecificContext()
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
            _consoleState.OutputBuffer.Add(message);
        }
    }
}