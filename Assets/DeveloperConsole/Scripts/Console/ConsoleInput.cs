using System;
using UnityEngine;

namespace DeveloperConsole
{
    public class ConsoleInput : IInputSource
    {
        public event Action<string> InputSubmitted;

        private string _inputBuffer = "";
        
        public void Update(Event current)
        {
            GUI.SetNextControlName("ConsoleInput");
            _inputBuffer = GUILayout.TextField(_inputBuffer);
            GUI.FocusControl("ConsoleInput");

            if (current.type != EventType.KeyDown || current.character is not ('\n' or '\r')) return;
            
            current.Use();
            InputSubmitted?.Invoke(_inputBuffer);
            _inputBuffer = "";
        }
    }
}