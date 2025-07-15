using System;

namespace DeveloperConsole.Core.Kernel
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