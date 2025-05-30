using System;
using System.Reflection;

namespace DeveloperConsole
{
    public class KernelServiceProxy<T> : DispatchProxy where T : class
    {
        private Kernel _kernel;

        public void Setup(Kernel kernel) => _kernel = kernel;

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var liveInstance = _kernel.GetLiveInstance<T>();
            if (liveInstance == null)
                throw new ObjectDisposedException(typeof(T).Name, $"Service {typeof(T).Name} is no longer available.");

            return targetMethod.Invoke(liveInstance, args);
        }
    }
}