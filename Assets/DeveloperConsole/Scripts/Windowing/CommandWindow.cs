using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Core.Shell;
using DeveloperConsole.IO;
using UnityEngine;

namespace DeveloperConsole.Windowing
{
    public class CommandWindow : WindowBase, IPromptResponder, IOutputChannel
    {
        private List<string> _outputBuffer = new();
        private Vector2 _scrollPosition = Vector2.zero;
        private UserInterface _userInterface;

        private CancellationTokenSource _cancellationTokenSource;

        public CommandWindow(WindowConfig config) : base(config)
        {
            _userInterface = new UserInterface(this, this);
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

            // TODO: Add input here if needed.

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        public override void OnInput(Event current)
        {
            // TODO: Add input capture if needed.
        }

        public UserInterface GetInterface()
        {
            return _userInterface;
        }

        public void Write(string message)
        {
            _outputBuffer[^1] += message;
            _scrollPosition.y = float.MaxValue;
        }

        public void OverWrite(string message)
        {
            _outputBuffer[^1] = message;
            _scrollPosition.y = float.MaxValue;
        }

        public void WriteLine(string line)
        {
            if (string.IsNullOrEmpty(line)) return;
            _outputBuffer.Add(line);
            _scrollPosition.y = float.MaxValue;
        }

        public Task<object> HandlePrompt(Prompt prompt, CancellationToken cancellationToken)
        {
            // No prompting allowed currently. This should never be called.
            throw new System.NotImplementedException();
        }

        public CancellationToken GetCommandCancellationToken()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            return _cancellationTokenSource.Token;
        }

        public ShellSignalHandler GetSignalHandler()
        {
            throw new System.NotImplementedException();
        }

        public void SetPromptHeader(string header)
        {
            throw new System.NotImplementedException();
        }

        private void SignalHandler(ShellSignal signal)
        {
            throw new System.NotImplementedException();
        }
    }
}
