using DeveloperConsole.Command;

namespace DeveloperConsole.IO
{
    /// <summary>
    /// An input representing a command
    /// </summary>
    public interface ICommandInput : IInput
    {
        public ICommandResolver GetResolver();
    }
}
