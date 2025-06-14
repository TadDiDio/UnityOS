using System;

namespace DeveloperConsole.Core
{    
    /// <summary>
    /// Interface for tickable kernel applications.
    /// </summary>
    public interface IKernelApplication : IDisposable
    {
        /// <summary>
        /// A regular update tick.
        /// </summary>
        public void Tick();
    }
}