using UnityEngine;
using DeveloperConsole.Command;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DeveloperConsole
{
    [Restrict(UnityEnvironment.EditMode)]
    [Command("play", "Enters play mode.")]
    public class PlayCommand : SimpleCommand
    {
        protected override CommandOutput Execute(SimpleContext context)
        {
            #if UNITY_EDITOR
            EditorApplication.isPlaying = true;
            #endif
            return new CommandOutput("Entering playmode...");
        }
    }

    [ConfirmBeforeExecuting("Are you sure you want to exit the player?")]
    [Restrict(UnityEnvironment.Runtime)]
    [Command("stop", "Exits play mode or the application.")]
    public class StopCommand : SimpleCommand
    {
        protected override CommandOutput Execute(SimpleContext context)
        {
            if (context.Environment is UnityEnvironment.Build)
            {
                Application.Quit();
                return new();
            }

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            return new();
        }
    }

    [Restrict(UnityEnvironment.Runtime)]
    [Command("stopf", "Exits play mode or the application without confirming.")]
    public class StopFCommand : SimpleCommand
    {
        protected override CommandOutput Execute(SimpleContext context)
        {
            if (context.Environment is UnityEnvironment.Build)
            {
                Application.Quit();
                return new();
            }

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            return new();
        }
    }
}
