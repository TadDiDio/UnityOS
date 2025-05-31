using UnityEngine;

namespace DeveloperConsole
{
    public class ConsoleInput : IInputSource
    {
        private bool _inputAvailable;
        private string _inputBuffer = "";
        
        public void OnGUI()
        {
            GUI.SetNextControlName("ConsoleInput");
            _inputBuffer = GUILayout.TextField(_inputBuffer);
            GUI.FocusControl("ConsoleInput");
            
            var e = Event.current;
            if (e.type == EventType.KeyDown && e.character is '\n' or '\r')
            {
                _inputAvailable = true;
                e.Use();
            }
        }

        public bool InputAvailable() => _inputAvailable;

        public string GetInput()
        {
            string result = _inputBuffer;
            
            _inputBuffer = "";
            _inputAvailable = false;
            
            return result;
        }
    }
}