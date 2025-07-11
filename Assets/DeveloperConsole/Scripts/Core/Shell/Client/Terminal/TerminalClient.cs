using System;
using System.Collections.Generic;
using DeveloperConsole.Core.Shell;
using DeveloperConsole.IO;
using UnityEngine;
using DeveloperConsole.Windowing;

namespace DeveloperConsole
{
    public class TerminalClient : IWindow, IShellClient
    {
        public Action<IInput> OnInputSubmitted { get; set; }
        private Type _nextInputType;

        private Vector2 _scrollPosition = Vector2.zero;

        private TerminalInputBuffer _inputBuffer = new();
        private List<string> _outputBuffer = new();


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
            foreach (string line in _outputBuffer)
            {
                GUILayout.Label(line, TerminalGUIStyle.DefaultStyle());
            }

            // Input field inline with output
            GUILayout.BeginHorizontal();

            GUILayout.Label("> ", TerminalGUIStyle.Prompt(), GUILayout.ExpandWidth(false));
            GUI.SetNextControlName("TerminalInputField");
            _inputBuffer.CurrentBuffer = GUILayout.TextField(_inputBuffer.CurrentBuffer, TerminalGUIStyle.InputField());

            GUI.FocusControl("TerminalInputField");

            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        public void OnInput(Event current)
        {
            // Raw key presses. Will refactor to generic input provider system
            if (current.type is not EventType.KeyDown) return;

            _scrollPosition.y = float.MaxValue;

            if (current.keyCode == KeyCode.UpArrow)
            {
                _inputBuffer.LessRecent();
                current.Use();
            }
            else if (current.keyCode == KeyCode.DownArrow)
            {
                _inputBuffer.MoreRecent();
                current.Use();
            }
            else if (current.keyCode is KeyCode.Return or KeyCode.KeypadEnter)
            {
                SubmitInput();
                current.Use();
            }
        }

        private void SubmitInput()
        {
            IInput input = _nextInputType switch
            {
                {} t when t == typeof(PromptResponseInput) => new PromptResponseInput(_inputBuffer.CurrentBuffer),
                _ => new TextCommandInput(_inputBuffer.CurrentBuffer)
            };
            _nextInputType = typeof(TextCommandInput);

            // TODO: support dynamic prompt selection
            WriteLine($"> {_inputBuffer.CurrentBuffer}");

            _inputBuffer.PushHistory();

            OnInputSubmitted?.Invoke(input);
        }

        public void Write(string token)
        {
            _outputBuffer[^1] += token;
        }

        public void WriteLine(string line)
        {
            if (string.IsNullOrEmpty(line)) return;
            _outputBuffer.Add(line);
        }

        public bool CanHandlePrompt(PromptRequest request) => true;


        public void PromptedFor<T>(PromptRequest request) where T : IInput
        {
            _nextInputType = typeof(T);
        }

        public ShellSession.SignalHandler GetSignalHandler() => SignalHandler;

        private void SignalHandler(ShellSignal signal)
        {
            if (signal is ClearSignal)
            {
                _outputBuffer.Clear();
            }
        }
    }
}
