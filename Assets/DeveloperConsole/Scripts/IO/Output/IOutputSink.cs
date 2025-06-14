namespace DeveloperConsole.IO
{
    /// <summary>
    /// Receives output.
    /// </summary>
    public interface IOutputSink
    {
        /// <summary>
        /// Receives an output message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ReceiveOutput(IOutputMessage message);
    }
}