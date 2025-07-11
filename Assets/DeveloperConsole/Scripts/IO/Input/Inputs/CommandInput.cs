using DeveloperConsole.Command;

namespace DeveloperConsole.IO
{
    public class CommandInput : ICommandInput
    {
        private ICommand _command;
        public CommandInput(ICommand command) => _command = command;
        public ICommandResolver GetResolver() => new CommandCommandResolver(_command);
    }
}
