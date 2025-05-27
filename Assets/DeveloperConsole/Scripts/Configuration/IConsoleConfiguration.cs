using UnityEngine;

namespace DeveloperConsole
{
    [CreateAssetMenu(fileName = "ConsoleConfiguration", menuName = "Developer Console/Configuration")]
    public class ConsoleConfiguration : ScriptableObject
    {
        public bool AutoDetectSetup = true;
        public ManualConsoleSetup Setup;

        public static ConsoleConfiguration Default()
        {
            ConsoleConfiguration config = CreateInstance<ConsoleConfiguration>();

            config.AutoDetectSetup = true;
            config.Setup = null;

            return config;
        }
    }
}