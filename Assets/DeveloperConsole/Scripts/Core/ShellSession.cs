using System;
using System.Threading.Tasks;
using DeveloperConsole.IO;

namespace DeveloperConsole.Core
{
    public class ShellSession
    {
        private bool _waitingForInput;
        private TaskCompletionSource<TextInput> _inputTcs;
        
        // Owns interaction and session context
        
        // Must track if locked or not
        // Must buffer input
        // Must keep aliases
        // Must track waiting for input
        
        // Should keep environment variables
        
        // Input sources must know their session so input manager
        // can direct input to session rather than shell.

        public bool WaitingForInput() => _waitingForInput;

        public void ReceiveInput(TextInput input)
        {
            if (_inputTcs != null && !_inputTcs.Task.IsCompleted)
            {
                _inputTcs.SetResult(input);
                _inputTcs = null;
            }

            _waitingForInput = false;
        }

        public async Task<TextInput> WaitForInput()
        {
            _waitingForInput = true;

            // If already waiting, prevent overwriting an existing TCS.
            if (_inputTcs != null && !_inputTcs.Task.IsCompleted)
            {
                throw new InvalidOperationException("Already waiting for input");
            }

            _inputTcs = new TaskCompletionSource<TextInput>(TaskCreationOptions.RunContinuationsAsynchronously);
            return await _inputTcs.Task;
        }
    }
}