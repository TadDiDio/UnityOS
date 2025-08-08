using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command;
using DeveloperConsole.Core.Shell;

namespace DeveloperConsole
{
    public class TerminalClient : IHumanInterface
    {
        private enum TerminalState
        {
            Busy,
            General,
            ChoicePrompt,
            CommandPrompt
        }

        // Visual related state
        private List<string> _outputBuffer = new();
        private Vector2 _scrollPosition = Vector2.zero;
        private TerminalHistoryBuffer _historyBuffer = new();

        // Processing related state
        private DefaultCommandBatcher _batcher = new();
        private PromptChoice[] _choices;
        private TerminalState _state = TerminalState.General;
        private TaskCompletionSource<object> _promptResponseSource;
        private CancellationTokenSource _cancellationSource;

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
            if (_state is not TerminalState.Busy)
            {
                RenderInput();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void RenderInput()
        {
            switch (_state)
            {
                case TerminalState.General:
                case TerminalState.CommandPrompt:
                    GUILayout.BeginHorizontal();

                    GUILayout.Label("> ", TerminalGUIStyle.Prompt(), GUILayout.ExpandWidth(false));
                    GUI.SetNextControlName("TerminalInputField");
                    _historyBuffer.CurrentBuffer = GUILayout.TextField(_historyBuffer.CurrentBuffer, TerminalGUIStyle.InputField());

                    GUI.FocusControl("TerminalInputField");

                    GUILayout.EndHorizontal();
                    break;
                case TerminalState.ChoicePrompt:
                    GUILayout.BeginHorizontal();

                    foreach (var choice in _choices)
                    {
                        if (GUILayout.Button(choice.Label))
                        {
                            SubmitChoiceInput(choice);
                            break;
                        }
                    }

                    GUILayout.EndHorizontal();
                    break;
            }
        }

        public void OnInput(Event current)
        {
            // Raw key presses. Will refactor to generic input provider system
            if (current.type is not EventType.KeyDown) return;

            _scrollPosition.y = float.MaxValue;

            if (current.keyCode == KeyCode.UpArrow)
            {
                _historyBuffer.LessRecent();
                current.Use();
            }
            else if (current.keyCode == KeyCode.DownArrow)
            {
                _historyBuffer.MoreRecent();
                current.Use();
            }
            else if (current.keyCode is KeyCode.Return or KeyCode.KeypadEnter)
            {
                if (_promptResponseSource == null) return;

                SubmitInput();
                current.Use();
            }
            else if (current.keyCode is KeyCode.C && current.modifiers.HasFlag(EventModifiers.Control))
            {
                _cancellationSource?.Cancel();
            }
        }


        private void SubmitChoiceInput(PromptChoice choice)
        {
            WriteLine($"> {choice.Label}");
            _promptResponseSource.SetResult(choice.Value);
        }

        private void SubmitInput()
        {
            string input = _historyBuffer.CurrentBuffer;
            _historyBuffer.PushHistory();

            // TODO: support dynamic prompt selection
            WriteLine($"> {input}");

            if (_state is TerminalState.CommandPrompt)
            {
                _promptResponseSource.SetResult(_batcher.GetBatch(input));
            }
            else _promptResponseSource.SetResult(input);
        }

        public void Write(string token)
        {
            _outputBuffer[^1] += token;
            _scrollPosition.y = float.MaxValue;
        }

        public void WriteLine(string line)
        {
            if (string.IsNullOrEmpty(line)) return;
            _outputBuffer.Add(line);
            _scrollPosition.y = float.MaxValue;
        }


        public async Task<object> HandlePrompt(Prompt prompt, CancellationToken cancellationToken)
        {
            if (_promptResponseSource != null)
            {
                string error = $"There was already an active prompt when another was requested on {GetType().Name}. Only one can be open at once.";
                throw new InvalidOperationException(error);
            }

            _state = prompt.Kind switch
            {
                PromptKind.Choice => TerminalState.ChoicePrompt,
                PromptKind.Command => TerminalState.CommandPrompt,
                _ => TerminalState.General
            };

            var tcs = new TaskCompletionSource<object>();
            _promptResponseSource = tcs;

            await using var reg = cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));

            DisplayPromptOnce(prompt);

            try
            {
                var result = await tcs.Task;
                return result;
            }
            finally
            {
                _promptResponseSource = null;
                _state = TerminalState.Busy;
            }
        }

        public CancellationToken GetCommandCancellationToken()
        {
            _cancellationSource = new CancellationTokenSource();
            return _cancellationSource.Token;
        }


        private void DisplayPromptOnce(Prompt prompt)
        {
            switch (prompt.Kind)
            {
                case PromptKind.General:
                    WriteLine(prompt.Message);
                    break;
                case PromptKind.Choice:
                    WriteLine(prompt.Message);
                    _choices = (PromptChoice[])prompt.Metadata[PromptMetaKeys.Choices];
                    break;
                case PromptKind.Confirmation:
                    WriteLine($"{prompt.Message} (y/n)");
                    break;
            }
        }

        // TODO: Convert signal handlers to data driven.
        public ShellSignalHandler GetSignalHandler() => SignalHandler;
        private void SignalHandler(ShellSignal signal)
        {
            if (signal is ClearSignal)
            {
                _outputBuffer.Clear();
            }
        }
    }
}
