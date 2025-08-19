namespace DeveloperConsole.IO
{
    public interface IOutputChannel
    {
        /// <summary>
        /// Writes to the current output line.
        /// </summary>
        /// <param name="message">The message to add.</param>
        /// <param name="overwrite">True to overwrite the current output line.</param>
        public void Write(string message, bool overwrite = false);

        /// <summary>
        /// Writes a new line.
        /// </summary>
        /// <param name="line">The line to write.</param>
        public void WriteLine(string line);
    }
}
