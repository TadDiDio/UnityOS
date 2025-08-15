using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Command;
using DeveloperConsole.Core.Shell;
using DeveloperConsole.Persistence;
using UnityEngine;

namespace DeveloperConsole.Windowing
{
    // TODO: Redo all of this its almost an exact duplicate of terminal client
    public class CommandWindow : WindowBase, IShellClient
    {
        private List<string> _outputBuffer = new();
        private Vector2 _scrollPosition = Vector2.zero;
        private TerminalHistoryBuffer _historyBuffer;
        private DefaultCommandBatcher _batcher = new();
        private PromptChoice[] _choices;
        private TerminalState _state = TerminalState.General;
        private TaskCompletionSource<object> _promptResponseSource;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _bufferFocus;
        private string _header = "> ";

        private string _promptHeader;

        private Guid id = Guid.NewGuid();
        public CommandWindow(WindowConfig config) : base(config)
        {
            OnClose += CancelCommand;
            _historyBuffer = new TerminalHistoryBuffer(new PersistentHistoryContainer(new List<string>()));
        }

        private void CancelCommand(IWindow window)
        {
            _cancellationTokenSource?.Cancel();
        }

        private enum TerminalState
        {
            Busy,
            General,
            ChoicePrompt,
            CommandPrompt
        }

        public IOContext GetIOContext()
        {
            return new IOContext(this, this, new SignalEmitter(this));
        }


        protected override void DrawContent(Rect areaRect)
        {
            // Cut off the scroll bar
            Rect windowRect = new Rect(areaRect.x, areaRect.y, areaRect.width - 12, areaRect.height - 12);
            bool clicked = Event.current.type is EventType.MouseDown && windowRect.Contains(Event.current.mousePosition);

            if (clicked)
            {
                // Literally no idea why this has to be cleared but it prevents toggling control to scene
                GUI.FocusControl("");
                _bufferFocus = true;
            }

            GUILayout.BeginVertical();
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));

            // Print each output line
            foreach (string line in _outputBuffer)
            {
                GUILayout.Label(line);
            }

            // Input field inline with output
            if (_state is not TerminalState.Busy)
            {
                RenderInput();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void RenderInput()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(_header, GUILayout.ExpandWidth(false));
            GUI.SetNextControlName(id.ToString());
            _historyBuffer.CurrentBuffer = GUILayout.TextField(_historyBuffer.CurrentBuffer);

            if (_historyBuffer.CurrentBuffer.EndsWith("/"))
            {
                _historyBuffer.CurrentBuffer = _historyBuffer.CurrentBuffer[..^1];
                OnHide?.Invoke(this);
            }

            if (_bufferFocus)
            {
                if (GUI.GetNameOfFocusedControl() != id.ToString())
                {
                    GUI.FocusControl(id.ToString());
                }
                else _bufferFocus = false;
            }

            GUILayout.EndHorizontal();
        }

        public override void OnInput(Event current)
        {
            void CursorToEnd()
            {
                if (GUI.GetNameOfFocusedControl() == id.ToString())
                {
                    var textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                    textEditor.cursorIndex = textEditor.selectIndex = _historyBuffer.CurrentBuffer.Length;
                }
            }


            if (current.type is not (EventType.KeyDown or EventType.Used)) return;
            if (current.keyCode is not KeyCode.None) _scrollPosition.y = float.MaxValue;

            if (current.keyCode == KeyCode.UpArrow)
            {
                _historyBuffer.LessRecent();
                current.Use();

                CursorToEnd();
            }
            else if (current.keyCode == KeyCode.DownArrow)
            {
                _historyBuffer.MoreRecent();
                current.Use();
            }
            else if (current.keyCode is KeyCode.Return or KeyCode.KeypadEnter)
            {
                Log.Info("Command window got enter key");

                if (_promptResponseSource == null) return;
                if (_state is TerminalState.ChoicePrompt) SubmitChoiceInput(); else SubmitInput();
                current.Use();
            }
            else if (current.keyCode is KeyCode.C && current.modifiers.HasFlag(EventModifiers.Control))
            {
                _cancellationTokenSource?.Cancel();
            }
        }

        private void SubmitChoiceInput()
        {
            string input = _historyBuffer.CurrentBuffer;
            if (!int.TryParse(input, out int number) || number < 1 || number > _choices.Length)
            {
                _historyBuffer.PushHistory();
                WriteLine($"{_header}{input}");
                WriteLine("Invalid choice, please select a number that is in range of the choices:");
                return;
            }

            var choice = _choices[number - 1];

            _historyBuffer.PushHistory();
            WriteLine($"{_header}{choice.Label}");

            _promptResponseSource.SetResult(choice.Value);
        }

        private void SubmitInput()
        {
            string input = _historyBuffer.CurrentBuffer;
            _historyBuffer.PushHistory();

            WriteLine($"{_header}{input}");

            if (_state is TerminalState.CommandPrompt)
            {
                var batch = _batcher.GetBatch(input);
                batch.AllowPrompting = true;
                _promptResponseSource.SetResult(batch);
            }
            else _promptResponseSource.SetResult(input);
        }

        public void SetPromptHeader(string header)
        {
            _header = header;
        }

        public void Write(string message, bool overwrite)
        {
            if (overwrite) _outputBuffer[^1] = message;
            else _outputBuffer[^1] += message;

            _scrollPosition.y = float.MaxValue;
        }


        public void WriteLine(string line)
        {
            Log.Info("Writing: " + line);
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

        public CancellationToken GetPromptCancellationToken()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            return _cancellationTokenSource.Token;
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

                    int num = 1;
                    foreach (var choice in _choices)
                    {
                        WriteLine($"{num++}. {choice.Label}");
                    }

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
