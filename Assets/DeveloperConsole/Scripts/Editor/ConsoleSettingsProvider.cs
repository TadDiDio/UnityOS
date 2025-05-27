using System.Linq;
using UnityEditor;

namespace DeveloperConsole.Editor
{
    public static class ConsoleSettingsProvider
    {
        private const string ResourcePath = "ConsoleConfigurationSelector";
        
        [SettingsProvider]
        public static SettingsProvider CreateConsoleSettingsProvider()
        {
            
            var provider = new SettingsProvider("Project/Developer Console", SettingsScope.Project)
            {
                label = "Developer Console",
                guiHandler = (searchContext) =>
                {
                    var configs = AssetDatabase.FindAssets("t:ConsoleConfiguration")
                        .Select(guid => AssetDatabase.LoadAssetAtPath<ConsoleConfiguration>(AssetDatabase.GUIDToAssetPath(guid)))
                        .Where(config => config)
                        .ToList();

                    if (configs.Count == 0)
                    {
                        EditorGUILayout.HelpBox("No ConsoleConfiguration assets found.\nCreate one via Right-click → Create → Developer Console → Configuration", MessageType.Warning);
                        return;
                    }

                    var selector = ConsoleConfigLoader.GetConsoleConfigurationSelector();

                    EditorGUI.BeginChangeCheck();

                    var newConfig = (ConsoleConfiguration)EditorGUILayout.ObjectField(
                        "Console Configuration",
                        selector.SelectedConfiguration,
                        typeof(ConsoleConfiguration),
                        false // disallow scene objects
                    );

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(selector, "Change Console Configuration");
                        selector.SelectedConfiguration = newConfig;
                        EditorUtility.SetDirty(selector);
                        AssetDatabase.SaveAssets();
                    }

                    if (!selector.SelectedConfiguration) return;
                    
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Selected Config Preview", EditorStyles.boldLabel);
                        
                    SerializedObject serializedConfig = new SerializedObject(selector.SelectedConfiguration);
                    SerializedProperty property = serializedConfig.GetIterator();

                    if (property.NextVisible(true))
                    {
                        do
                        {
                            if (property.name == "m_Script") continue; // Skip script reference
                            EditorGUILayout.PropertyField(property, true);
                        }
                        while (property.NextVisible(false));
                    }

                    serializedConfig.ApplyModifiedProperties();
                },
                
                keywords = new[] {"console", "developer", "terminal", "tools"}
            };
            
            return provider;
        }
    }
}