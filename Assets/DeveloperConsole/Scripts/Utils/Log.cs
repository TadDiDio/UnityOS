using System;
using UnityEngine;

namespace DeveloperConsole
{
    // TODO: Add handlers here to output to any global error log for the console
    /// <summary>
    /// A logging wrapper for the system.
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Info(object message)
        {
            Debug.Log(message);
        }


        /// <summary>
        /// Logs a warning.
        /// </summary>
        /// <param name="message">The warning.</param>
        public static void Warning(object message)
        {
            Debug.LogWarning(message);
        }


        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="message">The error.</param>
        public static void Error(object message)
        {
            Debug.LogError(message);
        }


        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public static void Exception(Exception exception)
        {
            Debug.LogException(exception);
        }
    }
}
