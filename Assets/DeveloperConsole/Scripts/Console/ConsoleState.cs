using System;
using System.Collections.Generic;

namespace DeveloperConsole
{
    [Serializable]
    public class ConsoleState
    {
        public List<string> OutputBuffer = new();
        public List<string> CommandHistory = new();

        public void AddCommandHistory(string input)
        {
            if (CommandHistory.Count > 0 && CommandHistory[^1] == input) return;
        
            CommandHistory.Add(input);
            // TODO: Add max history check from preferences
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
            // TODO: Check max output buffer
            
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