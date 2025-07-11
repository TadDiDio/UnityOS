using DeveloperConsole.Command;

namespace DeveloperConsole.IO
{
    public class TextCommandInput : ICommandInput
    {
        private string _rawInput;

        public TextCommandInput(string rawInput) => _rawInput = rawInput;
        public ICommandResolver GetResolver() => new TextCommandResolver(_rawInput);
    }
}
