using UnityEngine;

namespace DeveloperConsole
{
    // TODO: Move some of this to user configs
    public static class ConsoleGUIStyle
    {
        private static GUIStyle _default;
        private static GUIStyle _prompt;
        private static GUIStyle _inputField;
        
        public static GUIStyle DefaultStyle()
        {
            if (_default != null) return _default;
            
            Font _font = Resources.Load<Font>("Fonts/jetbrains-mono.regular");
            _default = new GUIStyle
            {
                font = _font,
                richText = true,
                fontSize = 14,
                normal =
                {
                    textColor = new Color(0.9f, 0.95f, 1f)
                },
                padding = new RectOffset(4, 4, 2, 2),
                margin = new RectOffset(0, 0, 0, 0),
                wordWrap = false,
                alignment = TextAnchor.MiddleLeft,
            };
            
            return _default;
        }

        public static GUIStyle Prompt()
        {
            if (_prompt != null) return _prompt;

            _prompt = new GUIStyle(DefaultStyle());
            _prompt.margin.right = 0;
            _prompt.padding.right = 0;
            
            return _prompt;   
        }
        
        public static GUIStyle InputField()
        {
            if (_inputField != null) return _inputField;

            _inputField = new GUIStyle(DefaultStyle());
            _inputField.margin.left = 0;
            _inputField.padding.left = 0;
            
            return _inputField;   
        }
    }
}