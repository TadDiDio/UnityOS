namespace DeveloperConsole.IO
{
    public interface IOutputChannel
    {
        /// <summary>
        /// Writes to the current output line.
        /// </summary>
        /// <param name="message">The message to add.</param>
        public void Write(string message);

        /// <summary>
        /// Overwrites the current output line with the message.
        /// </summary>
        /// <param name="message">The message to replace with.</param>
        public void OverWrite(string message);

        /// <summary>
        /// Writes a new line.
        /// </summary>
        /// <param name="line">The line to write.</param>
        public void WriteLine(string line);
    }
}
