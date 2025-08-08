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
        /// User-defined success of the command execution.
        /// </summary>
        public readonly Status Status;


        /// <summary>
        /// Creates an empty output.
        /// </summary>
        /// <param name="status">User-defined success status of the command.</param>
        public CommandOutput(Status status = Status.Success)
        {
            Message = "";
            Status = status;
        }


        /// <summary>
        /// Creates an output with a message.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="status">User-defined success status of the command.</param>
        public CommandOutput(string message, Status status = Status.Success)
        {
            Message = message;
            Status = status;
        }


        /// <summary>
        /// Creates an output message with each line on a new line.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <param name="status">User-defined success status of the command.</param>
        public CommandOutput(IEnumerable<string> lines, Status status = Status.Success)
        {
            Message = string.Join(Environment.NewLine, lines);
            Status = status;
        }


        /// <summary>
        /// Creates on output message from a result.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="status">User-defined success status of the command.</param>
        public CommandOutput(object result, Status status = Status.Success)
        {
            Message = result.ToString();
            Status = status;
        }
    }
}
