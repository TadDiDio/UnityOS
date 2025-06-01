using UnityEngine;
using System.Collections.Generic;

namespace DeveloperConsole
{
    public class ConsoleOutput : IOutputSink
    {
        private List<string> _outputBuffer;
        private GUIStyle _style;
        private Font _font;
        
        public ConsoleOutput(List<string> outputBuffer)
        {
            _outputBuffer = outputBuffer;
            _font = Resources.Load<Font>("Fonts/jetbrains-mono.regular");
        }

        public void OnGUI(ref Vector2 scrollPosition)
        {
            // TODO: Move this to a config
            if (_style == null)
            {
                _style = new GUIStyle(GUI.skin.label);
                _style.font = _font;
                _style.richText = true;
                _style.fontSize = 12;
            }
            
            
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            foreach (var output in _outputBuffer)
            {
                GUILayout.Label(output, _style);
            }
            GUILayout.EndScrollView();
        }
        
        public void ReceiveOutput(string message)
        {
            _outputBuffer.Add(message);
        }
    }
}