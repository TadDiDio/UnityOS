namespace DeveloperConsole.Command
{
    /// <summary>
    /// The output from a command.
    /// </summary>
    public class CommandOutput
    {
        /// <summary>
        /// The message.
        /// </summary>
        public string Message;
        
        
        /// <summary>
        /// The level of this output.
        /// </summary>
        public CommandOutputLogLevel Level;
        
        
        /// <summary>
        /// Creates an empty output.
        /// </summary>
        public CommandOutput() => Message = ""; 
        
        
        /// <summary>
        /// Creates an output with a message.
        /// </summary>
        /// <param name="message">The message</param>
        public CommandOutput(string message) => Message = message;
    }

    /// <summary>
    /// Defines possible command output log levels.
    /// </summary>
    public enum CommandOutputLogLevel
    {
        Info,
        Warning,
        Error
    }
}