using System;

namespace DeveloperConsole
{
    public interface IInputSource
    {
        /// <summary>
        /// This event must be raised when an input is submitted.
        /// </summary>
        public event Action<string> InputSubmitted;
    }
}