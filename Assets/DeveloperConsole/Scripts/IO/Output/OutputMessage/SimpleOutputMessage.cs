namespace DeveloperConsole.IO
{
    public class SimpleOutputMessage : IOutputMessage
    {
        public string Message;
        
        public SimpleOutputMessage(string message) => Message = message;
    }
}