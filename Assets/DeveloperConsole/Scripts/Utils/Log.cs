using System;
using UnityEngine;

namespace DeveloperConsole
{
    // TODO: Add handlers here to output to any global error log for the console
    public static class Log
    {
        public static void Info(string message)
        {
            Debug.Log(message);
        }

        public static void Warning(string message)
        {
            Debug.LogWarning(message);
        }

        public static void Error(string message)
        {
            Debug.LogError(message);
        }
        
        public static void Exception(Exception exception)
        {
            Debug.LogException(exception);
        }
    }
}