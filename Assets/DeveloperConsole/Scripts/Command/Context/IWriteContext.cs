using DeveloperConsole.IO;

namespace DeveloperConsole.Command
{
    public interface IWriteContext { }
    public class WriteContext : IWriteContext
    {
        private IOutputChannel _output;

        public WriteContext(IOutputChannel channel) { _output = channel; }

        /// <summary>
        /// Writes a line to the output channel.
        /// </summary>
        /// <param name="line">The line to write.</param>
        public void WriteLine(string line)
        {
            _output.WriteLine(line);
        }

        /// <summary>
        /// Writes to the output channel.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="overwrite">Whether to overwrite the current line.</param>
        public void Write(string message, bool overwrite = false)
        {
            _output.Write(message, overwrite);
        }
    }
}
