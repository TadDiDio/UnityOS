using DeveloperConsole.Command;
using DeveloperConsole.Core;

namespace DeveloperConsole.IO
{
    /// <summary>
    /// A string based input.
    /// </summary>
    public class TextInput : IInput
    {
        /// <summary>
        /// The input string.
        /// </summary>
        public readonly string Input;
        
        public ShellSession ShellSession { get; }
        
        
        /// <summary>
        /// Creates a new text input.
        /// </summary>
        /// <param name="session">The shell session associated with the source that created this input.</param>
        /// <param name="input">The string input.</param>
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