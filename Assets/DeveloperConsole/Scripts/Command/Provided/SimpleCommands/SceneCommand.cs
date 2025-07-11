using System;
using DeveloperConsole.Command;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace DeveloperConsole
{
    [Command("scene", "Manages scenes.")]
    public class SceneCommand : SimpleCommand
    {
        [Positional(0, "The name of the scene to load.")] private string sceneName;

        [Switch('a', "Should the scene be loaded additively?")]
        private bool additive;
        
        protected override CommandOutput Execute(CommandContext context)
        {
            // If in the editor, use the editor scene tool
            if (context.Environment is UnityEnvironment.EditMode)
            {
#if UNITY_EDITOR
                string scenePath = FindScenePathByName();
                if (!string.IsNullOrEmpty(scenePath))
                {
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                    return new CommandOutput($"Opened scene: {scenePath}");
                }
                return new CommandOutput($"Failed to open scene: {scenePath}. Ensure that this scene exists.");
#endif
            }
            
            // Else use the regular scene manager
            try
            {
                LoadSceneMode mode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
                SceneManager.LoadScene(sceneName, mode);
                return new CommandOutput($"Opened scene: {sceneName}");
            }
            catch (Exception)
            {
                return new CommandOutput($"Failed to open scene: {sceneName}. Ensure that this scene exists and is in the build list.");
            }
        }
        
        private string FindScenePathByName()
        {
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.path.EndsWith($"{sceneName}.unity", StringComparison.OrdinalIgnoreCase))
                    return scene.path;
            }
            return null;
        }
    }
    
    [Subcommand("list", "Lists the scenes available.", typeof(SceneCommand))]
    public class ListScenesCommand : SimpleCommand
    {
        protected override CommandOutput Execute(CommandContext context)
        {
            throw new NotImplementedException();
        }
    }
    
    [Subcommand("reload", "Reloads the active scene.", typeof(SceneCommand))]
    public class ReloadSceneCommand : SimpleCommand
    {
        protected override CommandOutput Execute(CommandContext context)
        {
            throw new NotImplementedException();
        }
    }
}