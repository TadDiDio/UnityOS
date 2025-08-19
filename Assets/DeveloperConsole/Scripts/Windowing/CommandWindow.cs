using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Core.Shell;
using UnityEngine;

namespace DeveloperConsole.Windowing
{
    // TODO: Redo all of this its almost an exact duplicate of terminal client
    public class CommandWindow : WindowBase, IShellClient
    {
        private List<string> _outputBuffer = new();
        private Vector2 _scrollPosition = Vector2.zero;
        private PromptChoice[] _choices;
        private TaskCompletionSource<object> _promptResponseSource;
        private CancellationTokenSource _cancellationTokenSource;

        private string _promptHeader;

        public CommandWindow(WindowConfig config) : base(config)
        {
            OnClose += CancelCommand;
        }

        private void CancelCommand(IWindow window)
        {
            _cancellationTokenSource?.Cancel();
        }

        protected override void DrawContent(Rect areaRect)
        {
            GUILayout.BeginVertical();
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));

            // Print each output line
            foreach (string line in _outputBuffer)
            {
                GUILayout.Label(line);
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        public override void OnInput(Event current)
        {
            // Nothing
        }


        public void SetPromptHeader(string header)
        {
        }

        public void Write(string message, bool overwrite)
        {
            if (overwrite) _outputBuffer[^1] = message;
            else _outputBuffer[^1] += message;

            _scrollPosition.y = float.MaxValue;
        }


        public void WriteLine(string line)
        {
            if (string.IsNullOrEmpty(line)) return;
            _outputBuffer.Add(line);
            _scrollPosition.y = float.MaxValue;
        }


        public Task<object> HandlePrompt<T>(Prompt<T> prompt, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Windows cannot be prompted yet.");
        }

        public CancellationToken GetPromptCancellationToken()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            return _cancellationTokenSource.Token;
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
