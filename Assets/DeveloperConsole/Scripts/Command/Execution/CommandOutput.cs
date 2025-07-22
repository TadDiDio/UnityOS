using System;
using System.Collections.Generic;

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
        /// Creates an empty output.
        /// </summary>
        public CommandOutput() => Message = "";


        /// <summary>
        /// Creates an output with a message.
        /// </summary>
        /// <param name="message">The message</param>
        public CommandOutput(string message) => Message = message;


        /// <summary>
        /// Creates an output message with each line on a new line.
        /// </summary>
        /// <param name="lines">The lines.</param>
        public CommandOutput(IEnumerable<string> lines) => Message = string.Join(Environment.NewLine, lines);


        /// <summary>
        /// Creates on output message from a result.
        /// </summary>
        /// <param name="result"></param>
        public CommandOutput(object result) => Message = result.ToString();
    }
}
