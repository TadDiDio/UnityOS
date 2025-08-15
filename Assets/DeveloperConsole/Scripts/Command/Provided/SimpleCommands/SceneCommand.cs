using System;
using System.IO;
using System.Linq;
using DeveloperConsole.Command;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace DeveloperConsole
{
    /// <summary>
    /// Command to manage scenes: open, close, list, reload.
    /// Supports both editor and runtime modes.
    /// </summary>
    [Command("scene", "Manages scenes.")]
    public class SceneCommand : SimpleCommand
    {
        [Positional(0, "The name of the scene to load.")]
        private string sceneName;

        [Switch('a', "Should the scene be loaded additively?")]
        private bool additive;

        protected override CommandOutput Execute(SimpleCommandContext context)
        {
            if (context.Environment is UnityEnvironment.EditMode)
            {
#if UNITY_EDITOR
                string scenePath = FindScenePathByName(sceneName);
                if (string.IsNullOrEmpty(scenePath))
                {
                    return new CommandOutput($"Failed to open scene: {sceneName}. Scene not found in Assets.");
                }

                OpenSceneMode mode = additive ? OpenSceneMode.Additive : OpenSceneMode.Single;
                EditorSceneManager.OpenScene(scenePath, mode);
                return new CommandOutput($"Opened scene: {sceneName}");
#endif
            }
            else
            {
                if (!IsSceneInBuildSettings(sceneName))
                {
                    return new CommandOutput($"Failed to open scene: {sceneName}. Scene not included in Build Settings.");
                }

                LoadSceneMode mode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
                try
                {
                    SceneManager.LoadScene(sceneName, mode);
                    return new CommandOutput($"Opened scene: {sceneName}");
                }
                catch (Exception ex)
                {
                    return new CommandOutput($"Failed to open scene: {sceneName}. Exception: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Finds the full asset path of a scene in the Assets folder by its name.
        /// Editor only.
        /// </summary>
        private static string FindScenePathByName(string sceneName)
        {
#if UNITY_EDITOR
            foreach (var guid in AssetDatabase.FindAssets("t:Scene", new[] { "Assets" }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.Equals(Path.GetFileNameWithoutExtension(path), sceneName, StringComparison.OrdinalIgnoreCase))
                {
                    return path;
                }
            }
#endif
            return null;
        }

        /// <summary>
        /// Checks if the scene is included in Build Settings (runtime).
        /// </summary>
        private static bool IsSceneInBuildSettings(string sceneName)
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            for (int i = 0; i < sceneCount; i++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                string name = Path.GetFileNameWithoutExtension(path);
                if (string.Equals(name, sceneName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Lists available scenes in the project (edit mode) or build (runtime).
        /// </summary>
        [Command("list", "Lists the scenes available.")]
        public class ListScenesCommand : SimpleCommand
        {
            protected override CommandOutput Execute(SimpleCommandContext context)
            {
                string[] sceneNames;

                if (context.Environment is UnityEnvironment.EditMode)
                {
#if UNITY_EDITOR
                    sceneNames = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" })
                        .Select(guid => Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(guid)))
                        .Distinct()
                        .ToArray();
#else
                    sceneNames = Array.Empty<string>();
#endif
                }
                else
                {
                    int count = SceneManager.sceneCountInBuildSettings;
                    sceneNames = new string[count];
                    for (int i = 0; i < count; i++)
                    {
                        string path = SceneUtility.GetScenePathByBuildIndex(i);
                        sceneNames[i] = Path.GetFileNameWithoutExtension(path);
                    }
                }

                return new CommandOutput(sceneNames);
            }
        }

        /// <summary>
        /// Reloads the active scene.
        /// </summary>
        [Restrict(UnityEnvironment.Runtime)]
        [Command("reload", "Reloads the active scene.")]
        public class ReloadSceneCommand : SimpleCommand
        {
            [Switch('a', "Loads the scene additively.")]
            private bool additive;

            protected override CommandOutput Execute(SimpleCommandContext context)
            {
                string activeSceneName = SceneManager.GetActiveScene().name;
                LoadSceneMode mode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;

                try
                {
                    SceneManager.LoadScene(activeSceneName, mode);
                    return new CommandOutput($"Scene '{activeSceneName}' reloaded.");
                }
                catch (Exception ex)
                {
                    return new CommandOutput($"Failed to reload scene '{activeSceneName}'. Exception: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Closes an open scene.
        /// </summary>
        [Command("close", "Closes an open scene.")]
        public class CloseCommand : SimpleCommand
        {
            [Positional(0, "The name of the scene to close.")]
            private string sceneName;

            protected override CommandOutput Execute(SimpleCommandContext context)
            {
                if (string.IsNullOrWhiteSpace(sceneName))
                    return new CommandOutput("Scene name cannot be empty.");

                Scene sceneToClose = default;
                bool found = false;

                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene openScene = SceneManager.GetSceneAt(i);
                    if (string.Equals(openScene.name, sceneName, StringComparison.OrdinalIgnoreCase))
                    {
                        sceneToClose = openScene;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    return new CommandOutput($"Could not close scene: {sceneName}. Ensure that this scene is currently open.");
                }

                if (Application.isPlaying)
                {
                    SceneManager.UnloadSceneAsync(sceneToClose);
                    return new CommandOutput($"Unloading scene: {sceneName}");
                }

#if UNITY_EDITOR
                bool closed = EditorSceneManager.CloseScene(sceneToClose, true);
                if (closed)
                {
                    return new CommandOutput($"Closed scene: {sceneName}");
                }
                return new CommandOutput($"Could not close the scene {sceneName}. Possibly the only open scene.");
#endif
            }
        }
    }
}
