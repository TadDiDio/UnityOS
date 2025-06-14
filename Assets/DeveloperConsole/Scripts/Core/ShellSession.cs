using System;
using System.Threading.Tasks;
using DeveloperConsole.IO;

namespace DeveloperConsole.Core
{
    /// <summary>
    /// Owns the interaction and state context for a shell session.
    /// </summary>
    public class ShellSession
    {
        private bool _waitingForInput;
        private TaskCompletionSource<TextInput> _inputTcs;
        
        // Must track if locked or not
        // Must buffer input
        // Must keep aliases
        // Must track waiting for input
        
        // Should keep environment variables
        
        // Input sources must know their session so input manager
        // can direct input to session rather than shell.

        
        
        /// <summary>
        /// Is this session waiting for input?
        /// </summary>
        /// <returns>True if this session is waiting for input.</returns>
        public bool WaitingForInput() => _waitingForInput;

        
        /// <summary>
        /// Receives text input. System will ignore all other input types while waiting for input.
        /// </summary>
        /// <param name="input">The input.</param>
        public void ReceiveInput(TextInput input)
        {
            if (_inputTcs != null && !_inputTcs.Task.IsCompleted)
            {
                _inputTcs.SetResult(input);
                _inputTcs = null;
            }

            _waitingForInput = false;
        }

        
        /// <summary>
        /// The task to wait for input asynchronously.
        /// </summary>
        /// <returns>The text input.</returns>
        /// <exception cref="InvalidOperationException">Thrown if this session was already waiting.</exception>
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