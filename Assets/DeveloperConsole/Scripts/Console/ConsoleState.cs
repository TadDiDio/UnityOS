using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeveloperConsole
{
    [Serializable]
    public class ConsoleState
    {
        public List<string> OutputBuffer = new();
        public List<string> CommandHistory = new();

        // TODO: Set these via config. Makes sure to save updates to file so they are not overriden when file is read
        public int MaxOutputBufferSize = 250;
        public int MaxCommandHistorySize = 250;
        
        public void AddCommandHistory(string input)
        {
            if (CommandHistory.Count > 0 && CommandHistory[^1] == input) return;

            if (MaxCommandHistorySize < 0)
            {
                Debug.LogWarning("The max command history limit can't be less than 0. Setting to 250.");
                MaxCommandHistorySize = 250;
                JsonFileManager.Save(this);
            }
            while (CommandHistory.Count >= MaxCommandHistorySize) CommandHistory.RemoveAt(0);

            CommandHistory.Add(input);
        }

        public bool TryGetHistory(int index, out string history)
        {
            history = "";
            if (index <= 0 || index > CommandHistory.Count) return false;
            
            history = CommandHistory[^index];
            return true;
        }
        
        public void AddOutput(string output)
        {
            if (OutputBuffer.Count >= MaxOutputBufferSize) OutputBuffer.RemoveAt(0);
            OutputBuffer.Add(output);
        }

        public static ConsoleState Default()
        {
            ConsoleState state = new()
            {
                OutputBuffer = new List<string>(),
                CommandHistory = new List<string>()
            };

            return state;
        }
    }
}