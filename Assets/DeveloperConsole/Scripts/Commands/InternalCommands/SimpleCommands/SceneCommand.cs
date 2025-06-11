using System;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace DeveloperConsole
{
    [Command("scene", "Manages scenes.", true)]
    public class SceneCommand : SimpleCommand
    {
        [Description("The name of the scene to load.")]
        [PositionalArg(0)] private string sceneName;
        
        [Description("Should the scene be loaded additively?")]
        [SwitchArg("additive", 'a')]
        private bool additive;
        
        [Subcommand] private ListScenesCommand listSubcommand;
        [Subcommand] private ReloadSceneCommand reloadSceneCommand;
        
        protected override CommandResult Execute(CommandContext context)
        {
            // If in the editor, use the editor scene tool
            if (context.Environment is UnityEnvironment.EditMode)
            {
#if UNITY_EDITOR
                string scenePath = FindScenePathByName();
                if (!string.IsNullOrEmpty(scenePath))
                {
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                    return new CommandResult($"Opened scene: {scenePath}");
                }
                return new CommandResult($"Failed to open scene: {scenePath}. Ensure that this scene exists.");
#endif
            }
            
            // Else use the regular scene manager
            try
            {
                LoadSceneMode mode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
                SceneManager.LoadScene(sceneName, mode);
                return new CommandResult($"Opened scene: {sceneName}");
            }
            catch (Exception e)
            {
                return new CommandResult($"Failed to open scene: {sceneName}. Ensure that this scene exists and is in the build list.");
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
    
    [Command("list", "Lists the scenes available.", false)]
    public class ListScenesCommand : SimpleCommand
    {
        protected override CommandResult Execute(CommandContext context)
        {
            throw new NotImplementedException();
        }
    }
    
    [Command("reload", "Reloads the active scene.", false)]
    public class ReloadSceneCommand : SimpleCommand
    {
        protected override CommandResult Execute(CommandContext context)
        {
            throw new NotImplementedException();
        }
    }
}