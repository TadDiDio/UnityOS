using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DeveloperConsole.Windowing
{
    public static class ModernDarkGUISkin
    {
        private static GUISkin _skin = null;

        public static GUISkin Skin => _skin ?? CreateSkin();

        private static GUISkin CreateSkin()
        {
#if UNITY_EDITOR
            // Always use Game view skin as base, even in Scene view
            _skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Game);
#else
            _skin  = GUI.skin; // Runtime
#endif

            // TODO: Create custom skin.

            return _skin;
        }
    }
}
