using DeveloperConsole.Core;

namespace DeveloperConsole.IO
{
    public class InputMirrorMessage : IOutputMessage
    {
        public IShellSession Session { get; }
        public string Prompt;
        public string Message;

        public InputMirrorMessage(IShellSession session, string prompt, string message)
        {
            Session = session;
            Prompt = prompt;
            Message = message;
        }
    }
}