using UnityEngine;
using System.Collections.Generic;

namespace DeveloperConsole
{
    public class DefaultTerminalOutput : ITerminalApplication
    {
        private List<string> _outputBuffer; 
        private static Vector2 _scrollPosition = Vector2.zero;

        public DefaultTerminalOutput(List<string> outputBuffer)
        {
            _outputBuffer = outputBuffer;
        }
        
        public void ReceiveOutput(string message) => _outputBuffer.Add(message);
        
        public void OnGUI(GUIContext context)
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            foreach (var line in _outputBuffer) GUILayout.Label(line, context.Style);
            GUILayout.EndScrollView();

        }
        
        public void OnInput(string input)
        {
            _scrollPosition.y = float.MaxValue;
            ConsoleKernel.RunInput(input);
        }
    }
}