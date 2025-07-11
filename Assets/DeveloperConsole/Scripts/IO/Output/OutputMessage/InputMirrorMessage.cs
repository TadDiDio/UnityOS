using DeveloperConsole.Core.Shell;

namespace DeveloperConsole.IO
{
    public class InputMirrorMessage : IOutputMessage
    {
        public ShellSession Session { get; }
        public string _prompt;
        
        private string _message;

        public InputMirrorMessage(ShellSession session, string prompt, string message)
        {
            Session = session;
            _prompt = prompt;
            _message = message;
        }
        
        public string Message() => $"{_prompt} {_message}";
    }
}