using UnityEngine;
using System.Collections.Generic;

namespace DeveloperConsole
{
    public class ConsoleOutput : IOutputSink
    {
        private List<string> _outputBuffer;
        public ConsoleOutput(List<string> outputBuffer)
        {
            _outputBuffer = outputBuffer;
        }

        public void OnGUI(ref Vector2 scrollPosition)
        {
            // TODO: Move this to a config
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.richText = true;
            
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            foreach (var output in _outputBuffer)
            {
                GUILayout.Label(output, style);
            }
            GUILayout.EndScrollView();
        }
        
        public void ReceiveOutput(string message)
        {
            _outputBuffer.Add(message);
        }
    }
}