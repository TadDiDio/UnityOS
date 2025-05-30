using UnityEngine;
using System.Collections.Generic;

namespace DeveloperConsole
{
    public class KernelApplication : ITerminalApplication
    {
        private List<string> _outputBuffer; 
        private Vector2 _scrollPosition = Vector2.zero;

        public KernelApplication(List<string> outputBuffer)
        {
            _outputBuffer = outputBuffer ?? new List<string>();
        }
        
        public void ReceiveOutput(string message) => _outputBuffer.Add(message);
        
        public void OnGUI(GUIContext context)
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            foreach (var line in _outputBuffer) GUILayout.Label(line, context.Style);
            GUILayout.EndScrollView();
        }
        
        public void OnInputRecieved(string input)
        {
            _scrollPosition.y = float.MaxValue;
            Kernel.Instance.RunInput(input);
        }
    }
}