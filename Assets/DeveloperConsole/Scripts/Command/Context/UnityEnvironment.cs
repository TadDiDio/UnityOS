#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DeveloperConsole.Command
{
    /// <summary>
    /// The different environments in Unity.
    /// </summary>
    public enum UnityEnvironment
    {
        /// <summary>
        /// This command will only be available in a build.
        /// </summary>
        Build,

        /// <summary>
        /// This command is only available in the editor.
        /// </summary>
        Editor,

        /// <summary>
        /// This command is only available during edit mode.
        /// </summary>
        EditMode,

        /// <summary>
        /// This command is only available during play mode in the editor.
        /// </summary>
        PlayMode,

        /// <summary>
        /// This command is only available during play mode in either the editor or a build.
        /// </summary>
        Runtime
    }

    public static class UnityEnvironmentExtensions
    {
        /// <summary>
        /// Tells if a particular environment is available at the moment.
        /// </summary>
        /// <param name="environment">The environment to test.</param>
        /// <returns>True if it is available.</returns>
        public static bool IsAvailable(this UnityEnvironment environment)
        {
#if UNITY_EDITOR
            // If we are in play mode in the editor
            if (EditorApplication.isPlaying)
            {
                return environment is UnityEnvironment.PlayMode or UnityEnvironment.Editor or UnityEnvironment.Runtime;
            }

            // If we are in edit mode
            return environment is UnityEnvironment.Editor or UnityEnvironment.EditMode;
#else
            // If we are in a build
            return environment is UnityEnvironment.Build or UnityEnvironment.Runtime;
#endif
        }
    }
}
