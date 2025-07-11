using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeveloperConsole.IO;

namespace DeveloperConsole.Core.Shell
{
    /// <summary>
    /// Owns the interaction and state context for a shell session.
    /// </summary>
    public class ShellSession
    {
        /// <summary>
        /// True if this session is waiting for input.
        /// </summary>
        public bool WaitingForInput { get; private set; }
        
        /// <summary>
        /// The current type of input the session is waiting for.
        /// </summary>
        public Type RequestedInputType { get; private set; }
        
        /// <summary>
        /// The aliases this session is aware of.
        /// </summary>
        public Dictionary<string, List<string>> Aliases { get; private set; }
        
        /// <summary>
        /// The current session prompt.
        /// </summary>
        public string CurrentPrompt { get; set; } = ">";

        /// <summary>
        /// Tells if the session is accepting input or not.
        /// </summary>
        public bool IsLocked { get; set; }
        
        private Queue<IInput> _inputQueue = new();

        // TODO: Dont instantiate this here
        private PersistentSessionState _persistentSessionState = new();
        
        private TaskCompletionSource<IInput> _waitForInputSource;
        
        
        public List<string> CommandHistory => _persistentSessionState.CommandHistory;
        public List<string> OutputBuffer => _persistentSessionState.OutputBuffer;
        
        
        
        // Could keep environment variables
        
        
        
        /// Receives text input. System will ignore all other input types while waiting for input.
        /// </summary>
        /// <param name="input">The input.</param>
        public void ReceiveInput(IInput input)
        {
            if (_waitForInputSource != null && !_waitForInputSource.Task.IsCompleted)
            {
                _waitForInputSource.SetResult(input);
                _waitForInputSource = null;
                RequestedInputType = null;
            }

            WaitingForInput = false;
        }

        
        /// <summary>
        /// The task to wait for input asynchronously.
        /// </summary>
        /// <returns>The text input.</returns>
        /// <exception cref="InvalidOperationException">Thrown if this session was already waiting.</exception>
        public async Task<T> Prompt<T>(PromptContext context) where T : IInput
        {
            // If already waiting, prevent overwriting an existing TCS.
            if (_waitForInputSource != null && !_waitForInputSource.Task.IsCompleted)
            {
                throw new InvalidOperationException("Already waiting for input");
            }

            RequestedInputType = typeof(T);
            WaitingForInput = true;
            
            _waitForInputSource = new TaskCompletionSource<IInput>(TaskCreationOptions.RunContinuationsAsynchronously);
            var input = await _waitForInputSource.Task;

            if (input is T typed)
            {
                return typed;
            }
            throw new InvalidOperationException($"Received input is not of expected type {typeof(T).Name}");
        }


        /// <summary>
        /// Sets an alias.
        /// </summary>
        /// <param name="key">The alias</param>
        /// <param name="value">The value.</param>
        public void SetAlias(string key, List<string> value)
        {
            Aliases[key] = value;
        }

        
        /// <summary>
        /// Removes an alias.
        /// </summary>
        /// <param name="key">The aliase to remove.</param>
        public void RemoveAlias(string key)
        {
            Aliases.Remove(key);
        }
        
        
        /// <summary>
        /// Adds to the command history.
        /// </summary>
        /// <param name="input">The history to add.</param>
        public void AddCommandHistory(string input)
        {
            if (_persistentSessionState.CommandHistory.Count > 0 && 
                _persistentSessionState.CommandHistory[^1] == input) return;

            if (_persistentSessionState.MaxCommandHistorySize < 0)
            {
                Log.Warning("The max command history limit can't be less than 0. Setting to 250.");
                _persistentSessionState.MaxCommandHistorySize = 250;
            }

            while (_persistentSessionState.CommandHistory.Count >= _persistentSessionState.MaxCommandHistorySize)
            {
                _persistentSessionState.CommandHistory.RemoveAt(0);
            }

            _persistentSessionState.CommandHistory.Add(input);
        }

        
        /// <summary>
        /// Try to get the history at the given index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="history">The resulting history.</param>
        /// <returns>Whether it succeeded.</returns>
        public bool TryGetHistory(int index, out string history)
        {
            history = "";
            if (index <= 0 || index > _persistentSessionState.CommandHistory.Count) return false;
            
            history = _persistentSessionState.CommandHistory[^index];
            return true;
        }
        
        
        /// <summary>
        /// Adds to the output buffer.
        /// </summary>
        /// <param name="output">The output to add.</param>
        public void AddOutput(string output)
        {
            if (_persistentSessionState.OutputBuffer.Count >= _persistentSessionState.MaxOutputBufferSize)
            {
                _persistentSessionState.OutputBuffer.RemoveAt(0);
            }
            _persistentSessionState.OutputBuffer.Add(output);
        }
    }
}