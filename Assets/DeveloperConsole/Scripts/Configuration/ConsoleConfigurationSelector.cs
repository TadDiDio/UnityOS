#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

// TODO: All of this is invalid because the ConsoleConfiguration SO is now a class
namespace DeveloperConsole
{
    public class ConsoleConfigurationSelector : ScriptableObject
    {
        public ConsoleConfiguration SelectedConfiguration;
    }

    public static class ConsoleConfigLoader
    {
        private const string SelectorResourcePath = "ConsoleConfigurationSelector";
        private const string ResourcesPath = "Assets/DeveloperConsole/Resources";
        private const string SelectorAssetPath = "Assets/DeveloperConsole/Resources/ConsoleConfigurationSelector.asset";

        public static ConsoleConfigurationSelector GetConsoleConfigurationSelector()
        {
            var existing = Resources.Load<ConsoleConfigurationSelector>(SelectorResourcePath);
            if (existing) return existing;
            
            Debug.Log("ConsoleConfigurationSelector not found, creating one now. " +
                      "You should not delete or modify this asset, but you can move it to a different Resources folder.");
#if UNITY_EDITOR
            var selector = ScriptableObject.CreateInstance<ConsoleConfigurationSelector>();
            System.IO.Directory.CreateDirectory(ResourcesPath); // Ensure folder exists
            AssetDatabase.CreateAsset(selector, SelectorAssetPath);
            AssetDatabase.SaveAssets();
            Debug.Log("Created new ConsoleConfigurationSelector at " + ResourcesPath);
            return selector;
#else
            Debug.LogError("ConsoleConfigurationSelector is missing and cannot be created at runtime in a build. Please create one in the Editor.");
            return null;
#endif
        }
    }

}