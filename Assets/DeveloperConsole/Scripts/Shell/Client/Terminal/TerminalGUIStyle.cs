using UnityEngine;

namespace DeveloperConsole
{
    public static class TerminalGUIStyle
    {
        public static GUIStyle InputField() => Prompt();

        public static GUIStyle Prompt()
        {
            return new GUIStyle(GUI.skin.textField)
            {
                margin =
                {
                    right = 0
                },
                padding =
                {
                    right = 0
                }
            };
        }
    }
}
