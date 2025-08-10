using System;
using System.Collections.Generic;

namespace DeveloperConsole.Command
{
    /// <summary>
    /// Interface for command discovery strategies.
    /// </summary>
    public interface ICommandDiscoveryStrategy
    {
        /// <summary>
        /// Get all the command types the registry should know about.
        /// </summary>
        /// <returns>All command types.</returns>
        public List<Type> GetAllValidCommandTypes();
    }
}