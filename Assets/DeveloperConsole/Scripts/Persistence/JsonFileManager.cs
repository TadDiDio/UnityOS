using System.IO;
using UnityEngine;

namespace DeveloperConsole
{
    public static class JsonFileManager
    {
        // TODO: Add multiple scripts to split by responsibility
        private static readonly string FilePath = Path.Combine(Application.persistentDataPath, "console_state.json");
        
        public static ConsoleState Load()
        {
            if (!File.Exists(FilePath)) return new ConsoleState();

            try
            {
                var json = File.ReadAllText(FilePath);
                ConsoleState state = JsonUtility.FromJson<ConsoleState>(json);

                if (state != null) return state;
                
                Debug.LogError($"Can't load console state from {FilePath}");
                return new ConsoleState();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Failed to load console state: {e}");
                return new ConsoleState();
            }
        }

        public static void Save(ConsoleState state)
        {
            try
            {
                var json = JsonUtility.ToJson(state, prettyPrint: true);
                File.WriteAllText(FilePath, json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save console state: {e}");
            }
        }
    }
}