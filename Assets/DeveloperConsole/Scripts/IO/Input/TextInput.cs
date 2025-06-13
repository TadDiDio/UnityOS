using DeveloperConsole.Command;
using DeveloperConsole.Core;

namespace DeveloperConsole.IO
{
    public class TextInput : IInput
    {
        public readonly string Input;
        public ShellSession ShellSession { get; }
        
        public TextInput(ShellSession session, string input)
        {
            ShellSession = session;
            Input = input;
        }
        public CommandRequest GetCommandRequest()
        {
            return new CommandRequest(ShellSession, new TextCommandResolver(Input));
        }
    }
}