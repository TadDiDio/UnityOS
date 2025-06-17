using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeveloperConsole.IO;

namespace DeveloperConsole.Core
{
    /// <summary>
    /// Owns the interaction and state context for a shell session.
    /// </summary>
    public class ShellSession : IShellSession
    {
        public bool WaitingForInput { get; private set; }
        public Dictionary<string, List<string>> Aliases { get; private set; }
        
        private TaskCompletionSource<TextInput> _inputTcs;
        
        public List<string> OutputBuffer = new();
        public List<string> CommandHistory = new();

        // TODO: Set these via config. Makes sure to save updates to file so they are not overriden when file is read
        public int MaxOutputBufferSize = 250;
        public int MaxCommandHistorySize = 250;
        
        // TODO: Make this more protected.
        public string CurrentPrompt { get; set; } = ">";
        
        // TODO:
        // Must track if locked or not
        // Must buffer input
        // Must track waiting for input
        
        // Should keep environment variables
        
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

            WaitingForInput = false;
        }

        
        /// <summary>
        /// The task to wait for input asynchronously.
        /// </summary>
        /// <returns>The text input.</returns>
        /// <exception cref="InvalidOperationException">Thrown if this session was already waiting.</exception>
        public async Task<TextInput> WaitForInput()
        {
            WaitingForInput = true;

            // If already waiting, prevent overwriting an existing TCS.
            if (_inputTcs != null && !_inputTcs.Task.IsCompleted)
            {
                throw new InvalidOperationException("Already waiting for input");
            }

            _inputTcs = new TaskCompletionSource<TextInput>(TaskCreationOptions.RunContinuationsAsynchronously);
            return await _inputTcs.Task;
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
            if (CommandHistory.Count > 0 && CommandHistory[^1] == input) return;

            if (MaxCommandHistorySize < 0)
            {
                Log.Warning("The max command history limit can't be less than 0. Setting to 250.");
                MaxCommandHistorySize = 250;
            }
            while (CommandHistory.Count >= MaxCommandHistorySize) CommandHistory.RemoveAt(0);

            CommandHistory.Add(input);
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
            if (index <= 0 || index > CommandHistory.Count) return false;
            
            history = CommandHistory[^index];
            return true;
        }
        
        
        /// <summary>
        /// Adds to the output buffer.
        /// </summary>
        /// <param name="output">The output to add.</param>
        public void AddOutput(string output)
        {
            if (OutputBuffer.Count >= MaxOutputBufferSize) OutputBuffer.RemoveAt(0);
            OutputBuffer.Add(output);
        }
    }
}