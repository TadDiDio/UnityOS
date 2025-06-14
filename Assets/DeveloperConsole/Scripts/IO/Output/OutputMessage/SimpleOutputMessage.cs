using DeveloperConsole.Core;

namespace DeveloperConsole.IO
{
    /// <summary>
    /// A simple string message output.
    /// </summary>
    public class SimpleOutputMessage : IOutputMessage
    {
        public ShellSession Session { get; }
        
        
        /// <summary>
        /// The string message.
        /// </summary>
        public string Message;
        
        
        /// <summary>
        /// Creates a new simple message.
        /// </summary>
        /// <param name="session">The session this output came from.</param>
        /// <param name="message">The message.</param>
        public SimpleOutputMessage(ShellSession session, string message)
        {
            Session = session;
            Message = message;
        }
    }
}