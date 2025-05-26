using UnityEngine;

namespace DeveloperConsole
{
    public class KernelGUI : IConsoleInputSource
    {
        private bool _inputAvailable;
        private string _inputBuffer = "";
        
        public void Draw(GUIContext context, ITerminalApplication terminalApplication)
        {
            GUILayout.BeginArea(context.AreaRect, GUI.skin.window);
            
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