using System;
using System.Reflection;

namespace DeveloperConsole.Core.Kernel
{
    /// <summary>
    /// A proxy to a kernel service that gives weak references.
    /// </summary>
    /// <typeparam name="T">The interface type of the service.</typeparam>
    public class KernelServiceProxy<T> : DispatchProxy where T : class
    {
        private Kernel _kernel;

        
        /// <summary>
        /// Initializes this object by injecting the kernel.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public void Setup(Kernel kernel) => _kernel = kernel;

        
        /// <summary>
        /// Forwards method calls from this proxy to the actual service if it exists.
        /// </summary>
        /// <param name="targetMethod">The target call.</param>
        /// <param name="args">The method args.</param>
        /// <returns>The method return.</returns>
        /// <exception cref="ObjectDisposedException">Throws if the service is inactive.</exception>
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var liveInstance = _kernel.GetLiveInstance<T>();
            if (liveInstance == null)
                throw new ObjectDisposedException(typeof(T).Name, $"Service {typeof(T).Name} is no longer available.");

            return targetMethod.Invoke(liveInstance, args);
        }
    }
}