using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Core.Shell;
using DeveloperConsole.Persistence;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DeveloperConsole.Windowing
{
    public enum TerminalState
    {
        Busy,
        General,
        ChoicePrompt,
        CommandPrompt
    }

    public class TerminalClientModel : IShellClient
    {
        public event Action OnUpdate;

        public string PromptHeader;
        public PromptChoice[] CurrentChoices;
        public TerminalState CurrentState = TerminalState.General;
        public List<string> OutputBuffer = new();
        public string InputBuffer => _historyBuffer.CurrentBuffer;

        private TerminalHistoryBuffer _historyBuffer;

        private CancellationTokenSource _cancellationSource;
        private TaskCompletionSource<object> _promptResponseSource;

        public TerminalClientModel()
        {
            _historyBuffer = HistoryPersistence.GetInitial();
            Application.quitting += SaveHistory;
#if UNITY_EDITOR
            AssemblyReloadEvents.beforeAssemblyReload += SaveHistory;
            EditorApplication.quitting += SaveHistory;
#endif
        }

        private void SaveHistory()
        {
            HistoryPersistence.SaveHistory(_historyBuffer);
        }

        public async Task<object> HandlePrompt<T>(Prompt<T> prompt, CancellationToken cancellationToken)
        {
            if (_promptResponseSource != null)
            {
                string error = $"There was already an active prompt when another was requested on {GetType().Name}. Only one can be open at once.";
                throw new InvalidOperationException(error);
            }

            CurrentState = prompt.Kind switch
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
                CurrentState = TerminalState.Busy;
            }
        }

        private void DisplayPromptOnce<T>(Prompt<T> prompt)
        {
            switch (prompt.Kind)
            {
                case PromptKind.General:
                    WriteLine(prompt.Message);
                    break;
                case PromptKind.Choice:
                    WriteLine(prompt.Message);
                    CurrentChoices = (PromptChoice[])prompt.Metadata[PromptMetaKeys.Choices];

                    int num = 1;
                    foreach (var choice in CurrentChoices)
                    {
                        WriteLine($"{num++}. {choice.Label}");
                    }

                    break;
                case PromptKind.Confirmation:
                    WriteLine($"{prompt.Message} (y/n)");
                    break;
            }
        }

        public void SubmitChoice(int index)
        {
            string input = _historyBuffer.CurrentBuffer;
            if (index < 0 || index >= CurrentChoices.Length)
            {
                _historyBuffer.PushHistory();
                WriteLine($"{PromptHeader}{input}");
                WriteLine("Invalid choice, please select a number that is in range of the choices:");
                return;
            }

            var choice = CurrentChoices[index - 1];

            _historyBuffer.PushHistory();
            WriteLine($"{PromptHeader}{choice.Label}");

            _promptResponseSource.SetResult(choice.Value);
        }

        public void SubmitInput()
        {
            string input = _historyBuffer.CurrentBuffer;
            _historyBuffer.PushHistory();
            SetInputBuffer(string.Empty);

            WriteLine($"{PromptHeader}{input}");
            _promptResponseSource.SetResult(input);
        }

        public CancellationToken GetPromptCancellationToken()
        {
            if (!_cancellationSource?.IsCancellationRequested ?? false)
            {
                return _cancellationSource.Token;
            }

            _cancellationSource = new CancellationTokenSource();
            return _cancellationSource.Token;
        }

        public ShellSignalHandler GetSignalHandler()
        {
            void SignalHandler(ShellSignal signal)
            {
                if (signal is ClearSignal)
                {
                    ClearOutput();
                }
            }

            return SignalHandler;
        }

        public void SetPromptHeader(string header)
        {
            PromptHeader = header;
            OnUpdate?.Invoke();
        }

        public void SetInputBuffer(string input)
        {
            _historyBuffer.CurrentBuffer = input;
            OnUpdate?.Invoke();
        }

        public void Write(string message, bool overwrite = false)
        {
            if (OutputBuffer.Count == 0) OutputBuffer.Add(message);
            else if (overwrite) OutputBuffer[^1] = message;
            else OutputBuffer[^1] += message;

            OnUpdate?.Invoke();
        }

        public void WriteLine(string message)
        {
            OutputBuffer.Add(message);
            OnUpdate?.Invoke();
        }

        private void ClearOutput()
        {
            OutputBuffer.Clear();
            OnUpdate?.Invoke();
        }
    }
}
