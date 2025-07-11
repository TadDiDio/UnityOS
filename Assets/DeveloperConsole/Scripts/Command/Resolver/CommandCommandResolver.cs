using DeveloperConsole.Core.Shell;

namespace DeveloperConsole.Command
{
    public class CommandCommandResolver : ICommandResolver
    {
        private ICommand _command;

        public CommandCommandResolver(ICommand command) => _command = command;
        public CommandResolutionResult Resolve(ShellSession session) => CommandResolutionResult.Success(_command);
    }
}
