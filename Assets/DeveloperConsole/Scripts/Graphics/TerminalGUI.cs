using UnityEngine;

namespace DeveloperConsole
{
    public class TerminalGUI : IInputSource
    {
        private bool _inputAvailable;
        private string _inputBuffer = "";
        
        public void Draw(GUIContext context, ITerminalApplication terminalApplication)
        {
            // BUG: THIS IS CAUSE ARTIFACT
            GUILayout.BeginArea(context.AreaRect);
            
            terminalApplication.OnGUI(context);
            
            GUI.SetNextControlName("ConsoleInput");
            _inputBuffer = GUILayout.TextField(_inputBuffer, context.Style);
            GUI.FocusControl("ConsoleInput");
            
            var e = Event.current;
            if (e.type == EventType.KeyDown && e.character is '\n' or '\r')
            {
                _inputAvailable = true;
                e.Use();
            }
            
            GUILayout.EndArea();
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