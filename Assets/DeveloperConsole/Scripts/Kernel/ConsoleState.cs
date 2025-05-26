using System;
using UnityEngine;
using System.Collections.Generic;

namespace DeveloperConsole
{
    [Serializable]
    public class ConsoleState
    {
        public List<string> OutputBuffer = new();
        public List<string> CommandHistory = new();
        public GUIStyle ConsoleStyle = new(GUI.skin.label)
        {
            fontSize = 12,
            richText = true,
            wordWrap = true
        };

        public void AddHistory(string input)
        {
            if (CommandHistory.Count > 0 && CommandHistory[^1] == input) return;

            CommandHistory.Add(input);
            // TODO: Add max history check from preferences
        }

        public void AddOutput(string output)
        {
            // TODO: Check max output buffer
            
            OutputBuffer.Add(output);
        }
    }
}