using DeveloperConsole.Command;

namespace DeveloperConsole.Core.Shell
{
    public class ShellRequest
    {
        public ICommandResolver CommandResolver;
        public ShellSession Session;
        public IShellApplication Shell;

        public bool Windowed;
        public bool ExpandAliases;
        public bool NoPrompt; // TODO: Change this for prompt filter to allow some but not all

        private IHumanInterface _humanInterface;


        /// <summary>
        /// Overrides the human interface to allow something other than the spawning GUI to control it.
        /// </summary>
        /// <param name="humanInterface"></param>
        public void OverrideHumanInterface(IHumanInterface humanInterface)
        {
            _humanInterface = humanInterface;
        }
    }
}
