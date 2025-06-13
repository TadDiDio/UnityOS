using DeveloperConsole.Command;

namespace DeveloperConsole.Core
{
    public interface IShellApplication : IKernelApplication
    {
        public ShellSession CreateSession();
        public void HandleCommandRequestAsync(CommandRequest request);
    }
}