using System;

namespace DeveloperConsole
{    
    public interface IKernelApplication : IDisposable
    {
        public void Tick();
    }
}