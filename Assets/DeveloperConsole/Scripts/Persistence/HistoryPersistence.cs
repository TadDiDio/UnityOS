using System.IO;
using UnityEngine;

// TODO: This entire class needs to be removed and refactored. Currently overwrites history file because theres only one terminal
namespace DeveloperConsole.Persistence
{
    public static class HistoryPersistence
    {
        private static readonly string _savePath = Path.Combine(Application.persistentDataPath, "DevConsole/History/");

        public static TerminalHistoryBuffer GetInitial()
        {
            string path = GetPath();
            if (!File.Exists(path)) return new TerminalHistoryBuffer(null);

            try
            {
                var json = File.ReadAllText(path);
                var buffer = JsonUtility.FromJson<PersistentHistoryContainer>(json);

                if (buffer != null) return new TerminalHistoryBuffer(buffer);

                Log.Error($"Can't load console state from {path}");
                return new TerminalHistoryBuffer(null);
            }
            catch (System.Exception e)
            {
                Log.Warning($"Failed to load console state: {e}");
                return new TerminalHistoryBuffer(null);
            }
        }

        public static void SaveHistory(TerminalHistoryBuffer historyBuffer)
        {
            string path = GetPath();

            try
            {
                var container = new PersistentHistoryContainer(historyBuffer.GetBuffer());
                var json = JsonUtility.ToJson(container, prettyPrint: true);
                File.WriteAllText(path, json);
            }
            catch (System.Exception e)
            {
                Log.Error($"Failed to save console state: {e}");
            }
        }

        private static string GetPath()
        {
            string directory = Path.GetDirectoryName(_savePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

#if UNITY_EDITOR
            return Path.Combine(directory, "editor_save.json");
#else
            return Path.Combine(directory, "build_save.json");
#endif
        }
    }
}
