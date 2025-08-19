using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DeveloperConsole.Core.Shell;

namespace DeveloperConsole.Scripts.Command.Execution
{
    public class CommandPipe : IShellClient
    {
        private LinkedList<string> _buffer = new();
        private TaskCompletionSource<object> _promptResponseSource;

        public async Task<object> HandlePrompt<T>(Prompt<T> prompt, CancellationToken cancellationToken)
        {
            if (_promptResponseSource != null)
            {
                string error = $"There was already an active prompt when another was requested on {GetType().Name}. Only one can be open at once.";
                throw new InvalidOperationException(error);
            }

            var tcs = new TaskCompletionSource<object>();
            _promptResponseSource = tcs;

            await using var reg = cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));

            try
            {
                var result = await tcs.Task;
                return result;
            }
            finally
            {
                _promptResponseSource = null;
            }
        }

        private void AddItem(string item)
        {
            if (_promptResponseSource != null)
            {
                _promptResponseSource.TrySetResult(item);
            }
            else
            {
                _buffer.AddLast(item);
            }
        }

        public CancellationToken GetPromptCancellationToken()
        {
            throw new InvalidOperationException("Pipes cannot source cancellation tokens.");
        }

        public ShellSignalHandler GetSignalHandler()
        {
            void SignalHandler(ShellSignal signal)
            {
                if (signal is ClearSignal) _buffer.Clear();
            }

            return SignalHandler;
        }

        public void SetPromptHeader(string header)
        {
            // no-op
        }

        public void Write(string message, bool overwrite = false)
        {
            if (_buffer.Count == 0) _buffer.AddLast(message);
            else if (overwrite) _buffer.Last.Value = message;
            else _buffer.Last.Value += message;
        }

        public void WriteLine(string line)
        {
            AddItem(line);
        }
    }
}
