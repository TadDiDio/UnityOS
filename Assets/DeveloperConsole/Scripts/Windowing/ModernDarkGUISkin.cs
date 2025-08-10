using UnityEngine;

namespace DeveloperConsole.Windowing
{

    public static class ModernDarkGUISkin
    {
        private static GUISkin _skin;
        private static GUIStyle _baseStyle;
        private static Font _font;

        public static GUISkin Skin => _skin ?? CreateSkin();
        public static GUIStyle BaseStyle => _baseStyle ?? CreateBaseStyle();
        private static Font Font => _font ?? Resources.Load<Font>("Fonts/jetbrains-mono.regular");
        private static Color _textColor = new Color(0.9f, 0.95f, 1f);

        private static GUIStyle CreateBaseStyle()
        {
            _baseStyle = new GUIStyle
            {
                font = Font,
                richText = true,
                fontSize = 14,
                normal =
                {
                    textColor = _textColor
                },
                hover =
                {
                    textColor = _textColor
                },
                focused =
                {
                    textColor = _textColor
                },
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0),
                wordWrap = true,
                alignment = TextAnchor.MiddleLeft
            };

            return _baseStyle;
        }
        private static GUISkin CreateSkin()
        {
            _skin = ScriptableObject.CreateInstance<GUISkin>();

            _skin.font = Font;

            _skin.label = BaseStyle;

            _skin.window = WindowStyle();



            return _skin;
        }

        private static GUIStyle WindowStyle()
        {
            var normal = new GUIStyleState();
            var onNormal = new GUIStyleState();

            normal.textColor = _textColor;
            onNormal.textColor = _textColor;
            normal.background = Resources.Load<Texture2D>("Textures/normal");
            onNormal.background = Resources.Load<Texture2D>("Textures/on-normal");

            return new GUIStyle(BaseStyle)
            {
                normal = normal,
                onNormal = onNormal,
                padding = new RectOffset(5, 5, 5, 5),
                border = new RectOffset(5, 5, 5, 5),
                alignment = TextAnchor.UpperCenter,
            };
        }

        private static Texture2D CreateTexture(float greyscale)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, new Color(greyscale, greyscale, greyscale, 1));
            tex.Apply();
            return tex;
        }
        private static Texture2D CreateTexture(Color color)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }

        // TODO: Just for live editing, remove later
        public static void SetStyle(GUIStyle style)
        {
            Skin.window = style;
        }
    }
}
